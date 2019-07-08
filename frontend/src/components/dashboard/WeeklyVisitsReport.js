import React, { useState } from 'react';
import {
  XYPlot,
  LineSeries,
  VerticalGridLines,
  HorizontalGridLines,
  XAxis,
  YAxis
} from 'react-vis';
import { CSVLink } from 'react-csv';
import { ReportLayout, Table, Header, Card } from '../../ui';
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
            <XYPlot
              height={300}
              width={500}
              xType="ordinal"
              color="#1A70E3"
              getX={d => d.item}
              getY={d => d.count}
              animate
            >
              <VerticalGridLines />
              <HorizontalGridLines />
              <LineSeries data={visits} />
              <XAxis title="Week" style={{ fill: '#143740' }} />
              <YAxis title="Total Visitors" style={{ fill: '#143740' }} />
            </XYPlot>
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
