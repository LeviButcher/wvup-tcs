import React from 'react';
import { CSVLink } from 'react-csv';
import { Router } from '@reach/router';
import { ReportLayout, Table, Header, Card, LineChart } from '../../ui';
import StartToEndDateForm from '../../components/StartToEndDateForm';
import useApi from '../../hooks/useApi';

type Props = {
  navigate: any,
  '*': string
};

const WeeklyVisitsReport = ({ navigate, '*': unMatchedUri }: Props) => {
  const [start, end] = unMatchedUri.split('/');
  return (
    <ReportLayout>
      <StartToEndDateForm
        title="Weekly Visits Report"
        style={{ gridArea: 'form' }}
        onSubmit={({ startDate, endDate }) => {
          return Promise.resolve(navigate(`${startDate}/${endDate}`));
        }}
        initialValues={{ startDate: start, endDate: end }}
      />
      <Router primary={false} component={({ children }) => <>{children}</>}>
        {/* $FlowFixMe */}
        <VisitsResults path=":startDate/:endDate" />
      </Router>
    </ReportLayout>
  );
};

type VisitsResultsProps = {
  startDate: string,
  endDate: string
};

const VisitsResults = ({ startDate, endDate }: VisitsResultsProps) => {
  const [loading, visitsData] = useApi(
    `reports/weekly-visits?start=${startDate}&end=${endDate}`
  );
  return (
    <>
      {loading && <div>Loading...</div>}
      {!loading && visitsData && (
        <>
          <Card style={{ gridArea: 'chart' }}>
            <LineChart
              data={visitsData}
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
            <VisitsTable visits={visitsData} />
          </Card>
        </>
      )}
    </>
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
