import React, { useState } from 'react';
import { CSVLink } from 'react-csv';
import { ReportLayout, Table, Header, Card, LineChart } from '../../ui';
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
              .then(setPeakHours)
              .catch(e => setStatus({ msg: e.message }))
              .finally(() => setSubmitting(false));
          }}
          name="Peak Hours"
        />
        {peakHours && (
          <Card width="600px">
            <LineChart
              data={peakHours}
              x={d => d.item}
              y={d => d.count}
              title="Peak Hours"
              xLabel="Hour"
              yLabel="Total Visitors"
              labels={d => d.count}
            />
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
