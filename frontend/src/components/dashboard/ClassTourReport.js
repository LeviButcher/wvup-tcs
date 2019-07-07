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
import { Card, Header, Table, ReportLayout } from '../../ui';
import dataPointsConvertor from '../../utils/dataPointsConvertor';
import StartToEndDateForm from '../StartToEndDateForm';
import makeAsync from '../../utils/makeAsync';

const tourToXYPoint = dataPointsConvertor('name', 'totalTourists');

const toursData = [
  { name: 'Parkersburg South', totalTourists: 60 },
  { name: 'Ripley High', totalTourists: 45 },
  { name: 'Marietta', totalTourists: 16 },
  { name: 'Ravenswood High', totalTourists: 25 }
];

const getClassTourSum = makeAsync(1000, toursData);

const ClassTourReport = () => {
  const [tours, setTours] = useState();
  return (
    <ReportLayout>
      <div>
        <StartToEndDateForm
          onSubmit={(values, { setSubmitting }) => {
            getClassTourSum().then(res => {
              setTours(res);
              setSubmitting(false);
            });
          }}
          name="Class Tour"
        />
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

export default ClassTourReport;
