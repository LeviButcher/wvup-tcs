import React, { useState } from 'react';
import { CSVLink } from 'react-csv';
import { ReportLayout, Table, Header, Card, LineChart } from '../../ui';
import StartToEndDateForm from '../StartToEndDateForm';
import callApi from '../../utils/callApi';
import ensureReponseCode from '../../utils/ensureResponseCode';
import unwraptoJSON from '../../utils/unwrapToJSON';

const getVolunteerSum = (startDate, endDate) =>
  callApi(
    `${process.env.REACT_APP_BACKEND}reports/volunteers?start=${startDate}&end=${endDate}`,
    'GET',
    null
  );

const VolunteerReport = () => {
  const [volunteers, setVolunteers] = useState();
  return (
    <ReportLayout>
      <div>
        <StartToEndDateForm
          onSubmit={({ startDate, endDate }, { setSubmitting }) => {
            getVolunteerSum(startDate, endDate)
              .then(ensureReponseCode(200))
              .then(unwraptoJSON)
              .then(setVolunteers)
              .finally(() => setSubmitting(false));
          }}
          name="Volunteer Report"
        />
        {volunteers && (
          <Card width="600px">
            <LineChart
              data={volunteers}
              x={d => d.fullName}
              y={d => d.totalHours}
              xLabel="Email"
              yLabel="Total Hours"
              title="Volunteer Total Chart"
              labels={d => d.totalHours}
              domain={{ y: [0, 10] }}
            />
          </Card>
        )}
      </div>
      <div>{volunteers && <VolunteerTable volunteers={volunteers} />}</div>
    </ReportLayout>
  );
};

const VolunteerTable = ({ volunteers }) => {
  return (
    <Table>
      <caption>
        <Header>
          Volunteers Total Hours -{' '}
          <CSVLink data={volunteers} filename="classTourReport">
            Download
          </CSVLink>
        </Header>
      </caption>
      <thead>
        <tr>
          <td>Email</td>
          <td>Full Name</td>
          <td>Total Hours</td>
        </tr>
      </thead>
      <tbody>
        {volunteers.map(volunteer => (
          <tr key={volunteer.teacherEmail}>
            <td>{volunteer.teacherEmail}</td>
            <td>{volunteer.fullName}</td>
            <td>{volunteer.totalHours}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default VolunteerReport;
