import React, { useState } from 'react';
import { CSVLink } from 'react-csv';
import { Card, Header, Table, ReportLayout, BarChart } from '../../ui';
import StartToEndDateForm from '../StartToEndDateForm';
import { callApi, ensureResponseCode, unwrapToJSON } from '../../utils';

const getClassTourSum = (startDate, endDate) =>
  callApi(`reports/classtours?start=${startDate}&end=${endDate}`, 'GET', null);

const ClassTourReport = () => {
  const [tours, setTours] = useState();
  return (
    <ReportLayout>
      <StartToEndDateForm
        onSubmit={({ startDate, endDate }, { setSubmitting, setStatus }) => {
          getClassTourSum(startDate, endDate)
            .then(ensureResponseCode(200))
            .then(unwrapToJSON)
            .then(setTours)
            .catch(e => setStatus({ msg: e.message }))
            .finally(() => setSubmitting(false));
        }}
        name="Class Tour Report"
      />
      {tours && (
        <Card width="500px">
          <BarChart
            data={tours}
            x={d => d.name}
            y={d => d.students}
            title="Class Tour Chart"
            yLabel="# of Students"
            labels={d => d.students}
          />
        </Card>
      )}
      {tours && (
        <Card width="900px">
          <ClassTourSumTable classTours={tours} />
        </Card>
      )}
    </ReportLayout>
  );
};

const ClassTourSumTable = ({ classTours }) => {
  return (
    <Table>
      <caption>
        <Header>
          Total visitors per group -{' '}
          <CSVLink data={classTours} filename="classTourReport">
            Download
          </CSVLink>
        </Header>
      </caption>
      <thead>
        <tr>
          <td>Name</td>
          <td>Total Tourists</td>
        </tr>
      </thead>
      <tbody>
        {classTours.map(tour => (
          <tr key={tour.name}>
            <td>{tour.name}</td>
            <td>{tour.students}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default ClassTourReport;
