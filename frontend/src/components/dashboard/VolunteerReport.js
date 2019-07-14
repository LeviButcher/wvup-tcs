import React, { useState } from 'react';
import { CSVLink } from 'react-csv';
import { ReportLayout, Table, Header, Card, LineChart } from '../../ui';
import StartToEndDateForm from '../StartToEndDateForm';
import callApi from '../../utils/callApi';
import ensureReponseCode from '../../utils/ensureResponseCode';

const getVolunteerSum = (startDate, endDate) =>
  callApi(
    `${process.env.REACT_APP_BACKEND}reports/volunteers?start=${startDate}&end=${endDate}`,
    'GET',
    null
  );

const VolunteerReport = () => {
  const [volunteers, setVolunteers] = useState();
  console.log(volunteers);
  return (
    <ReportLayout>
      <div>
        <StartToEndDateForm
          onSubmit={({ startDate, endDate }, { setSubmitting }) => {
            getVolunteerSum(startDate, endDate)
              .then(ensureReponseCode(200))
              .then(async res => {
                const data = await res.json();
                setVolunteers(data);
              })
              .finally(() => setSubmitting(false));
          }}
          name="Voluteer"
        />
        {volunteers && (
          <Card width="600px">
            <LineChart
              data={volunteers}
              x={datum => datum.teacherEmail}
              y={datum => datum.signInTime}
              xLabel="Email"
              yLabel="Total Hours Volunteers"
              title="Volunteer Total Chart"
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
          Voluteers Total Hours -{' '}
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
