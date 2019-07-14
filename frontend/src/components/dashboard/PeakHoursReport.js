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
import ensureResponseCode from '../../utils/ensureResponseCode';
import unwrapToJSON from '../../utils/unwrapToJSON';

const getPeakHoursSum = (startDate, endDate) =>
  callApi(
    `${process.env.REACT_APP_BACKEND}reports/peakhours?start=${startDate}&end=${endDate}`,
    'GET',
    null
  );

const PeakHoursReport = () => {
  const [peakHours, setPeakHours] = useState();
  return (
    <ReportLayout>
      <div>
        <StartToEndDateForm
          onSubmit={({ startDate, endDate }, { setSubmitting, setStatus }) => {
            getPeakHoursSum(startDate, endDate)
              .then(ensureResponseCode(200))
              .then(unwrapToJSON)
              .then(res => {
                setPeakHours(res);
              })
              .catch(e => setStatus({ msg: e.message }))
              .finally(() => setSubmitting(false));
          }}
          name="Peak Hours"
        />
        {peakHours && (
          <Card width="600px">
            <XYPlot
              height={300}
              width={500}
              xType="ordinal"
              color="#1A70E3"
              getX={d => d.item}
              getY={d => d.count}
            >
              <VerticalGridLines />
              <HorizontalGridLines />
              <LineSeries data={peakHours} />
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
          <tr key={visit.item}>
            <td>{visit.item}</td>
            <td>{visit.count}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default PeakHoursReport;
