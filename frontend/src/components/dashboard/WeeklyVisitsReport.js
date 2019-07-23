import React, { useState } from 'react';
import { CSVLink } from 'react-csv';
import { ReportLayout, Table, Header, Card, LineChart } from '../../ui';
import StartToEndDateForm from '../StartToEndDateForm';
import { callApi, ensureResponseCode, unwrapToJSON } from '../../utils';

const getVisitsSum = (startDate, endDate) =>
  callApi(
    `reports/weekly-visits?start=${startDate}&end=${endDate}`,
    'GET',
    null
  );

const WeeklyVisitsReport = () => {
  const [visits, setVisits] = useState();
  return (
    <ReportLayout>
      <StartToEndDateForm
        onSubmit={(values, { setSubmitting }) => {
          const { startDate, endDate } = values;
          getVisitsSum(startDate, endDate)
            .then(ensureResponseCode(200))
            .then(unwrapToJSON)
            .then(setVisits)
            .catch(e => alert(e.message))
            .finally(() => setSubmitting(false));
        }}
        name="Weekly Visits Report"
      />
      {visits && visits.length > 0 && (
        <Card>
          <LineChart
            data={visits}
            x={d => d.item}
            y={d => d.count}
            xLabel="Week"
            yLabel="Total Visitors"
            title="Weekly Visits"
            labels={d => d.count}
            domain={{ x: [1, 2], y: [1, 2] }}
          />
        </Card>
      )}
      {visits && (
        <Card>
          <VisitsTable visits={visits} />
        </Card>
      )}
    </ReportLayout>
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
