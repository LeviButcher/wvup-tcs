import React from 'react';
import { CSVLink } from 'react-csv';
import { Router } from '@reach/router';
import { ReportLayout, Table, Header, Card, LineChart } from '../../ui';
import StartToEndDateForm from '../../components/StartToEndDateForm';
import LoadingContent from '../../components/LoadingContent';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';

type Props = {
  navigate: any,
  '*': string
};

const PeakHoursReport = ({ navigate, '*': unMatchedUri }: Props) => {
  const [start, end] = unMatchedUri.split('/');
  return (
    <ReportLayout>
      <StartToEndDateForm
        title="Peak Hours Report"
        style={{ gridArea: 'form' }}
        onSubmit={({ startDate, endDate }) => {
          return Promise.resolve(navigate(`${startDate}/${endDate}`));
        }}
        initialValues={{ startDate: start, endDate: end }}
      />
      <Router primary={false} component={({ children }) => <>{children}</>}>
        {/* $FlowFixMe */}
        <PeakHoursResult path=":startDate/:endDate" />
      </Router>
    </ReportLayout>
  );
};

type PeakHoursResultProps = {
  startDate: string,
  endDate: string
};

const PeakHoursResult = ({ startDate, endDate }: PeakHoursResultProps) => {
  const [loading, data, errors] = useApiWithHeaders(
    `reports/peakhours?start=${startDate}&end=${endDate}`
  );
  return (
    <LoadingContent loading={loading} data={data} errors={errors}>
      <Card width="600px" style={{ gridArea: 'chart' }}>
        <LineChart
          data={data.body}
          x={d => d.hour}
          y={d => d.count}
          title="Peak Hours"
          xLabel="Hour"
          yLabel="Total Visitors"
          labels={d => d.count}
          domain={{ x: [1, 2], y: [1, 2] }}
        />
      </Card>
      <Card width="800px" style={{ gridArea: 'table' }}>
        <PeakHoursTable peakHours={data.body} style={{ fontSize: '1.4rem' }} />
      </Card>
    </LoadingContent>
  );
};

const PeakHoursTable = ({ peakHours }) => {
  return (
    <Table>
      <caption>
        <Header>
          Peak Hours for Visitors -{' '}
          <CSVLink data={peakHours} filename="classTourReport">
            Download
          </CSVLink>
        </Header>
      </caption>
      <thead>
        <tr>
          <td>Hour</td>
          <td>Total Visitors</td>
        </tr>
      </thead>
      <tbody>
        {peakHours.map(visit => (
          <tr key={visit.hour}>
            <td>{visit.hour}</td>
            <td>{visit.count}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default PeakHoursReport;
