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
import dataPointsConvertor from '../../utils/dataPointsConvertor';
import { ReportLayout, Table, Header, Card } from '../../ui';
import StartToEndDateForm from '../StartToEndDateForm';

import makeAsync from '../../utils/makeAsync';

const volunteerData = [
  { email: 'gthompson@wvup.edu', fullName: 'Gary Thompson', totalHours: 25 },
  { email: 'mriddle@wvup.edu', fullName: 'M Riddle', totalHours: 55 },
  { email: 'calmond@wvup.edu', fullName: 'Charles Almond', totalHours: 30 }
];

const volunteerToXYPoint = dataPointsConvertor('email', 'totalHours');
const getVolunteerSum = makeAsync(1000, volunteerData);

const VolunteerReport = () => {
  const [volunteers, setVolunteers] = useState();
  return (
    <ReportLayout>
      <div>
        <StartToEndDateForm
          onSubmit={(values, { setSubmitting }) => {
            getVolunteerSum().then(res => {
              setVolunteers(res);
              setSubmitting(false);
            });
          }}
          name="Voluteer"
        />
        {volunteers && (
          <Card width="600px">
            <XYPlot height={300} width={500} xType="ordinal" color="#1A70E3">
              <VerticalGridLines />
              <HorizontalGridLines />
              <VerticalBarSeries data={volunteers.map(volunteerToXYPoint)} />
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
          <tr key={volunteer.email}>
            <td>{volunteer.email}</td>
            <td>{volunteer.fullName}</td>
            <td>{volunteer.totalHours}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default VolunteerReport;
