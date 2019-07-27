import React, { useState } from 'react';
import { CSVLink } from 'react-csv';
import { ReportLayout, Table, Header, Card, LineChart } from '../../ui';
import StartToEndDateForm from '../StartToEndDateForm';
import { callApi, ensureResponseCode, unwrapToJSON } from '../../utils';

const getPeakHoursSum = (startDate, endDate) =>
  callApi(`reports/peakhours?start=${startDate}&end=${endDate}`, 'GET', null);

const PeakHoursReport = () => {
  const [peakHours, setPeakHours] = useState();
  return (
    <ReportLayout>
      <StartToEndDateForm
        style={{ gridArea: 'form' }}
        onSubmit={({ startDate, endDate }, { setSubmitting, setStatus }) => {
          getPeakHoursSum(startDate, endDate)
            .then(ensureResponseCode(200))
            .then(unwrapToJSON)
            .then(setPeakHours)
            .catch(e => setStatus({ msg: e.message }))
            .finally(() => setSubmitting(false));
        }}
        name="Peak Hours Report"
      />
      {peakHours && peakHours.length > 0 && (
        <Card width="600px" style={{ gridArea: 'chart' }}>
          <LineChart
            data={peakHours}
            x={d => d.hour}
            y={d => d.count}
            title="Peak Hours"
            xLabel="Hour"
            yLabel="Total Visitors"
            labels={d => d.count}
            domain={{ x: [1, 2], y: [1, 2] }}
          />
        </Card>
      )}
      {peakHours && (
        <Card width="800px" style={{ gridArea: 'table' }}>
          <PeakHoursTable
            peakHours={peakHours}
            style={{ fontSize: '1.4rem' }}
          />
        </Card>
      )}
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
