import React from 'react';
import { CSVLink } from 'react-csv';
import { Router } from '@reach/router';
import ScaleLoader from 'react-spinners/ScaleLoader';
import { Card, Header, Table, ReportLayout, BarChart } from '../../ui';
import StartToEndDateForm from '../../components/StartToEndDateForm';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';

// Need to set up way to set StartDate and endDate in form on mount
const ClassTourReport = ({ navigate }) => {
  return (
    <ReportLayout>
      <StartToEndDateForm
        style={{ gridArea: 'form' }}
        onSubmit={({ startDate, endDate }, { setSubmitting }) => {
          navigate(`${startDate}/${endDate}`);
          setSubmitting(false);
        }}
        name="Class Tour Report"
      />
      <Router primary={false} component={({ children }) => <>{children}</>}>
        <ClassTourResult path=":startDate/:endDate" />
      </Router>
    </ReportLayout>
  );
};

const ClassTourResult = ({ startDate, endDate }) => {
  const [loading, data] = useApiWithHeaders(
    `reports/classtours?start=${startDate}&end=${endDate}`
  );
  return (
    <>
      <ScaleLoader sizeUnit="px" size={150} loading={loading} align="center" />
      {data && data.body && (
        <>
          <Card width="900px" style={{ gridArea: 'table' }}>
            <ClassTourSumTable classTours={data.body} />
          </Card>
          <Card width="600px" style={{ gridArea: 'chart' }}>
            <BarChart
              data={data.body}
              x={d => d.name}
              y={d => d.students}
              title="Class Tour Chart"
              yLabel="# of Students"
              labels={d => d.students}
              padding={{ left: 75, right: 75, top: 50, bottom: 50 }}
            />
          </Card>
        </>
      )}
    </>
  );
};

const ClassTourSumTable = ({ classTours }) => {
  return (
    <Table>
      <caption>
        <Header>
          Total visitors per group -{' '}
          <CSVLink data={classTours} filename="classTourReport">
            Download
          </CSVLink>
        </Header>
      </caption>
      <thead>
        <tr>
          <td>Name</td>
          <td>Total Tourists</td>
        </tr>
      </thead>
      <tbody>
        {classTours.map(tour => (
          <tr key={tour.name}>
            <td>{tour.name}</td>
            <td>{tour.students}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default ClassTourReport;
