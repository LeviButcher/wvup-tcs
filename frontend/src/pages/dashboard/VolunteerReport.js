import React from 'react';
import { CSVLink } from 'react-csv';
import { Router } from '@reach/router';
import { ReportLayout, Table, Header, Card, BarChart } from '../../ui';
import StartToEndDateForm from '../../components/StartToEndDateForm';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';
import LoadingContent from '../../components/LoadingContent';

type Props = {
  navigate: any,
  '*': any
};

const VolunteerReport = ({ navigate, '*': unMatchedUri }: Props) => {
  const [start, end] = unMatchedUri.split('/');
  return (
    <ReportLayout>
      <StartToEndDateForm
        title="Volunteer Report"
        style={{ gridArea: 'form' }}
        onSubmit={({ startDate, endDate }) => {
          return Promise.resolve(navigate(`${startDate}/${endDate}`));
        }}
        initialValues={{ startDate: start, endDate: end }}
      />
      <Router primary={false} component={({ children }) => <>{children}</>}>
        {/* $FlowFixMe */}
        <VolunteerResult path=":startDate/:endDate" />
      </Router>
    </ReportLayout>
  );
};

type VolunteerResultProps = {
  startDate: string,
  endDate: string
};

const VolunteerResult = ({ startDate, endDate }: VolunteerResultProps) => {
  const [loading, data, errors] = useApiWithHeaders(
    `reports/volunteers?start=${startDate}&end=${endDate}`
  );

  return (
    <LoadingContent loading={loading} data={data} errors={errors}>
      <Card width="600px" style={{ gridArea: 'chart' }}>
        <BarChart
          data={data.body}
          x={d => d.fullName}
          y={d => d.totalHours}
          yLabel="Total Hours"
          title="Volunteer Total Chart"
          labels={d => d.totalHours}
        />
      </Card>
      <Card width="900px" style={{ gridArea: 'table' }}>
        <VolunteerTable volunteers={data.body} />
      </Card>
    </LoadingContent>
  );
};

const VolunteerTable = ({ volunteers }) => (
  <Table>
    <caption>
      <Header>
        Volunteers Total Hours -{' '}
        <CSVLink data={volunteers} filename="classTourReport">
          Download
        </CSVLink>
      </Header>
    </caption>
    <thead>
      <tr>
        <td>Email</td>
        <td>Full Name</td>
        <td>Total Hours</td>
      </tr>
    </thead>
    <tbody>
      {volunteers.map(volunteer => (
        <tr key={volunteer.teacherEmail}>
          <td>{volunteer.teacherEmail}</td>
          <td>{volunteer.fullName}</td>
          <td>{volunteer.totalHours}</td>
        </tr>
      ))}
    </tbody>
  </Table>
);

export default VolunteerReport;
