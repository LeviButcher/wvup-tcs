import React from 'react';
import { CSVLink } from 'react-csv';
import { Router } from '@reach/router';
import { Card, Header, Table, ReportLayout, BarChart } from '../../ui';
import StartToEndDateForm from '../../components/StartToEndDateForm';
import LoadingContent from '../../components/LoadingContent';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';

const ClassTourReport = ({ navigate, '*': unMatchedUri }) => {
  const [start, end] = unMatchedUri.split('/');
  return (
    <ReportLayout>
      <StartToEndDateForm
        style={{ gridArea: 'form' }}
        title="Class Tour Report"
        onSubmit={({ startDate, endDate }, { setSubmitting }) => {
          navigate(`${startDate}/${endDate}`);
          setSubmitting(false);
        }}
        initialValues={{
          startDate: start,
          endDate: end
        }}
      />
      <Router primary={false} component={({ children }) => <>{children}</>}>
        <ClassTourResult path=":startDate/:endDate" />
      </Router>
    </ReportLayout>
  );
};

const ClassTourResult = ({ startDate, endDate }) => {
  const [loading, data, errors] = useApiWithHeaders(
    `reports/classtours?start=${startDate}&end=${endDate}`
  );
  return (
    <LoadingContent loading={loading} data={data} errors={errors}>
      <Card width="900px" style={{ gridArea: 'table' }}>
        <ClassTourSumTable classTours={data.body} />
      </Card>
      <Card width="600px" style={{ gridArea: 'chart' }}>
        <BarChart
          data={data.body}
          x={d => d.name}
          y={d => d.students}
          title="Class Tour Chart"
          horizontal
          yLabel="# of Students"
          labels={d => d.students}
          padding={{ left: 75, right: 75, top: 50, bottom: 50 }}
        />
      </Card>
    </LoadingContent>
  );
};

const ClassTourSumTable = ({ classTours }) => {
  return (
    <Table>
      <caption>
        <Header>
          Total tourists per Class Tour -{' '}
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
