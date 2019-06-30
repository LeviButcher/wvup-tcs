import React, { useState } from 'react';
import {
  XYPlot,
  VerticalBarSeries,
  VerticalGridLines,
  HorizontalGridLines,
  XAxis,
  YAxis
} from 'react-vis';
import styled from 'styled-components';
import { Formik, Form, Field } from 'formik';
import { CSVLink } from 'react-csv';
import { Card, Header, Button, Input, Table } from '../../ui';
import dataPointsConvertor from '../../utils/dataPointsConvertor';

const tourToXYPoint = dataPointsConvertor('name', 'totalTourists');

const toursData = [
  { name: 'Parkersburg South', totalTourists: 60 },
  { name: 'Ripley High', totalTourists: 45 },
  { name: 'Marietta', totalTourists: 16 },
  { name: 'Ravenswood High', totalTourists: 25 }
];

const getClassTourSum = () =>
  new Promise(res => setTimeout(() => res(toursData), 1000));

const ClassTourReport = () => {
  const [tours, setTours] = useState();
  return (
    <ReportLayout>
      <div>
        <Card>
          <Header>Class Tour Report</Header>
          <p>Enter begin and end date</p>
          <Formik
            onSubmit={(values, { setSubmitting }) => {
              getClassTourSum().then(res => {
                setTours(res);
                setSubmitting(false);
              });
            }}
          >
            {({ isSubmitting }) => (
              <Form>
                <Field
                  id="startDate"
                  type="date"
                  name="startDate"
                  component={Input}
                  label="start Date"
                />
                <Field
                  id="endDate"
                  type="date"
                  name="endDate"
                  component={Input}
                  label="end Date"
                />
                <Button align="right" intent="primary" disabled={isSubmitting}>
                  Run Report
                </Button>
              </Form>
            )}
          </Formik>
        </Card>
        {tours && (
          <Card width="600px">
            <XYPlot height={300} width={500} xType="ordinal" color="#1A70E3">
              <VerticalGridLines />
              <HorizontalGridLines />
              <VerticalBarSeries data={tours.map(tourToXYPoint)} />
              <XAxis title="Tour Groups" style={{ fill: '#143740' }} />
              <YAxis title="Total Tourists" style={{ fill: '#143740' }} />
            </XYPlot>
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
            <td>{tour.totalTourists}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

const ReportLayout = styled.div`
  display: grid;
  grid-template: 'form report' 1fr / auto 1fr;
  grid-gap: 30px;
  align-items: flex-start;
`;

export default ClassTourReport;
