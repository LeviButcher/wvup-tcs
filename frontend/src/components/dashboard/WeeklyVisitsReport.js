import React, { useState } from 'react';
import { CSVLink } from 'react-csv';
import { ReportLayout, Table, Header, Card, LineChart } from '../../ui';
import StartToEndDateForm from '../StartToEndDateForm';
import callApi from '../../utils/callApi';
import ensureReponseCode from '../../utils/ensureResponseCode';
import unwraptoJSON from '../../utils/unwrapToJSON';

const getVisitsSum = (startDate, endDate) =>
  callApi(
    `${process.env.REACT_APP_BACKEND}reports/weekly-visits?start=${startDate}&end=${endDate}`,
    'GET',
    null
  );

const WeeklyVisitsReport = () => {
  const [visits, setVisits] = useState();
  return (
    <ReportLayout>
      <div>
        <StartToEndDateForm
          onSubmit={(values, { setSubmitting }) => {
            const { startDate, endDate } = values;
            getVisitsSum(startDate, endDate)
              .then(ensureReponseCode(200))
              .then(unwraptoJSON)
              .then(setVisits)
              .catch(e => alert(e.message))
              .finally(() => setSubmitting(false));
          }}
          name="Weekly Visits"
        />
        {visits && (
          <Card width="600px">
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
      </div>
      <div>{visits && <VisitsTable visits={visits} />}</div>
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
