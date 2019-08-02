import React from 'react';
import { CSVLink } from 'react-csv';
import { Router } from '@reach/router';
import { ReportLayout, Table, Header, Card, LineChart } from '../../ui';
import StartToEndDateForm from '../../components/StartToEndDateForm';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';
import LoadingContent from '../../components/LoadingContent';

const WeeklyVisitsReport = ({ navigate, '*': unMatchedUri }) => {
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
        name="Weekly Visits Report"
      />
      <Router primary={false} component={({ children }) => <>{children}</>}>
        <VisitsResults path=":startDate/:endDate" />
      </Router>
    </ReportLayout>
  );
};

const VisitsResults = ({ startDate, endDate }) => {
  const [loading, data, errors] = useApiWithHeaders(
    `reports/weekly-visits?start=${startDate}&end=${endDate}`
  );
  return (
    <LoadingContent loading={loading} data={data} errors={errors}>
      <Card style={{ gridArea: 'chart' }}>
        <LineChart
          data={data.body}
          x={d => d.item}
          y={d => d.count}
          xLabel="Week"
          yLabel="Total Visitors"
          title="Weekly Visits"
          labels={d => d.count}
          domain={{ x: [1, 2], y: [1, 2] }}
        />
      </Card>
      <Card style={{ gridArea: 'table' }}>
        <VisitsTable visits={data.body} />
      </Card>
    </LoadingContent>
  );
};

const VisitsTable = ({ visits }) => {
  return (
    <Table>
      <caption>
        <Header>
          Weekly Total Visitors -{' '}
          <CSVLink data={visits} filename="classTourReport">
            Download
          </CSVLink>
        </Header>
      </caption>
      <thead>
        <tr>
          <td>Week</td>
          <td>Total Visitors</td>
        </tr>
      </thead>
      <tbody>
        {visits.map(visit => (
          <tr key={visit.item}>
            <td>{visit.item}</td>
            <td>{visit.count}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default WeeklyVisitsReport;
