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

const peakHoursData = [
  { hour: '8 - 8:59', totalVisits: 40 },
  { hour: '9 - 9:59', totalVisits: 80 },
  { hour: '10 - 10:59', totalVisits: 120 },
  { hour: '11 - 11:59', totalVisits: 67 },
  { hour: '12 - 12:59', totalVisits: 90 },
  { hour: '1 - 1:59', totalVisits: 90 }
];

const peakHoursToXYPoint = dataPointsConvertor('hour', 'totalVisits');
const getPeakHoursSum = makeAsync(1000, peakHoursData);

const PeakHoursReport = () => {
  const [peakHours, setPeakHours] = useState();
  return (
    <ReportLayout>
      <div>
        <StartToEndDateForm
          onSubmit={(values, { setSubmitting }) => {
            getPeakHoursSum().then(res => {
              setPeakHours(res);
              setSubmitting(false);
            });
          }}
          name="Peak Hours"
        />
        {peakHours && (
          <Card width="600px">
            <XYPlot height={300} width={500} xType="ordinal" color="#1A70E3">
              <VerticalGridLines />
              <HorizontalGridLines />
              <LineSeries data={peakHoursData.map(peakHoursToXYPoint)} />
              <XAxis title="Hour" style={{ fill: '#143740' }} />
              <YAxis title="Total Visitors" style={{ fill: '#143740' }} />
            </XYPlot>
          </Card>
        )}
      </div>
      <div>{peakHours && <PeakHoursTable peakHours={peakHours} />}</div>
    </ReportLayout>
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
          <tr key={visit.week}>
            <td>{visit.hour}</td>
            <td>{visit.totalVisits}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default PeakHoursReport;
