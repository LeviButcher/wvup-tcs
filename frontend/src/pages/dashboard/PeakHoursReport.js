import React from 'react';
import { CSVLink } from 'react-csv';
import { Router } from '@reach/router';
import { ReportLayout, Table, Header, Card, LineChart } from '../../ui';
import StartToEndDateForm from '../../components/StartToEndDateForm';
import LoadingContent from '../../components/LoadingContent';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';

const PeakHoursReport = ({ navigate, '*': unMatchedUri }) => {
  const [start, end] = unMatchedUri.split('/');
  return (
    <ReportLayout>
      <StartToEndDateForm
        style={{ gridArea: 'form' }}
        onSubmit={({ startDate, endDate }, { setSubmitting }) => {
          navigate(`${startDate}/${endDate}`);
          setSubmitting(false);
        }}
        startDate={start}
        endDate={end}
        name="Peak Hours Report"
      />
      <Router primary={false} component={({ children }) => <>{children}</>}>
        <PeakHoursResult path=":startDate/:endDate" />
      </Router>
    </ReportLayout>
  );
};

const PeakHoursResult = ({ startDate, endDate }) => {
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
