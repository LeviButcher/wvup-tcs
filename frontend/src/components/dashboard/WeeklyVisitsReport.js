import React, { useState } from 'react';
import { CSVLink } from 'react-csv';
import { ReportLayout, Table, Header, Card, LineChart } from '../../ui';
import StartToEndDateForm from '../StartToEndDateForm';
import callApi from '../../utils/callApi';

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
              .then(async res => {
                const data = await res.json();
                setVisits(data);
              })
              .catch(e => alert(e.message))
              .finally(() => setSubmitting(false));
          }}
          name="Weekly Visits"
        />
        {visits && (
          <Card width="600px">
            <LineChart
              title="Weekly Visits"
              data={visits}
              x={d => d.item}
              y={d => d.count}
              xLabel="Week"
              yLabel="Total Visitors"
              labels={d => d.count}
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
