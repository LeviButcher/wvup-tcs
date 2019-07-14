import React, { useState } from 'react';
import { CSVLink } from 'react-csv';
import { Card, Header, Table, ReportLayout, BarChart } from '../../ui';
import StartToEndDateForm from '../StartToEndDateForm';
import callApi from '../../utils/callApi';
import ensureResponseCode from '../../utils/ensureResponseCode';
import unwrapToJSON from '../../utils/unwrapToJSON';

const getClassTourSum = (startDate, endDate) =>
  callApi(
    `${process.env.REACT_APP_BACKEND}reports/classtours?start=${startDate}&end=${endDate}`,
    'GET',
    null
  );

const ClassTourReport = () => {
  const [tours, setTours] = useState();
  return (
    <ReportLayout>
      <div>
        <StartToEndDateForm
          onSubmit={({ startDate, endDate }, { setSubmitting, setStatus }) => {
            getClassTourSum(startDate, endDate)
              .then(ensureResponseCode(200))
              .then(unwrapToJSON)
              .then(setTours)
              .catch(e => setStatus({ msg: e.message }))
              .finally(() => setSubmitting(false));
          }}
          name="Class Tour"
        />
        {tours && (
          <Card width="700px">
            <BarChart
              data={tours}
              x={d => d.name}
              y={d => d.students}
              title="Class Tour Chart"
              xLabel="Name"
              yLabel="# of Students"
              labels={d => d.students}
            />
          </Card>
        )}
      </div>
      <div>
        {tours && (
          <>
            <ClassTourSumTable classTours={tours} />
          </>
        )}
      </div>
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
