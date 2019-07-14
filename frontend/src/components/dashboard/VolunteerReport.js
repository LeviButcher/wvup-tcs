import React, { useState } from 'react';
import {
  XYPlot,
  VerticalBarSeries,
  VerticalGridLines,
  HorizontalGridLines,
  XAxis,
  YAxis
} from 'react-vis';
import { CSVLink } from 'react-csv';
import { ReportLayout, Table, Header, Card } from '../../ui';
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
            <XYPlot
              height={300}
              width={500}
              xType="ordinal"
              color="#1A70E3"
              getX={datum => datum.teacherEmail}
              getY={datum => datum.signInTime}
            >
              <VerticalGridLines />
              <HorizontalGridLines />
              <VerticalBarSeries data={volunteers} />
              <XAxis title="Email" style={{ fill: '#143740' }} />
              <YAxis
                title="Total Hours Volunteered"
                style={{ fill: '#143740' }}
              />
            </XYPlot>
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
