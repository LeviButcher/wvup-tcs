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
import dataPointsConvertor from '../../utils/dataPointsConvertor';
import { ReportLayout, Table, Header, Card } from '../../ui';
import StartToEndDateForm from '../StartToEndDateForm';

import makeAsync from '../../utils/makeAsync';

const weeklyVisitsData = [
  { week: 1, totalVisits: 40 },
  { week: 2, totalVisits: 80 },
  { week: 3, totalVisits: 120 },
  { week: 4, totalVisits: 67 },
  { week: 5, totalVisits: 90 }
];

const visitsToXYPoint = dataPointsConvertor('week', 'totalVisits');
const getVisitsSum = makeAsync(1000, weeklyVisitsData);

const WeeklyVisitsReport = () => {
  const [visits, setVisits] = useState();
  return (
    <ReportLayout>
      <div>
        <StartToEndDateForm
          onSubmit={(values, { setSubmitting }) => {
            getVisitsSum().then(res => {
              setVisits(res);
              setSubmitting(false);
            });
          }}
          name="Weekly Visits"
        />
        {visits && (
          <Card width="600px">
            <XYPlot height={300} width={500} xType="ordinal" color="#1A70E3">
              <VerticalGridLines />
              <HorizontalGridLines />
              <LineSeries data={visits.map(visitsToXYPoint)} />
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
          <tr key={visit.week}>
            <td>{visit.week}</td>
            <td>{visit.totalVisits}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default WeeklyVisitsReport;
