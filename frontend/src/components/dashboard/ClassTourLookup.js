import React, { useState } from 'react';
import { Form, Field, Formik } from 'formik';
import { CSVLink } from 'react-csv';
import {
  Link,
  Input,
  Button,
  Card,
  Table,
  Header,
  ReportLayout
} from '../../ui';

import callApi from '../../utils/callApi';

const getClassTours = (startDate, endDate) =>
  callApi(`${process.env.REACT_APP_BACKEND}classtours/`, 'GET', null);

const deleteTour = id =>
  callApi(`${process.env.REACT_APP_BACKEND}classtours/${id}`, 'DELETE', null);

const ClassTourLookup = () => {
  const [tours, setTours] = useState();

  const loadTours = (startDate, endDate) =>
    getClassTours(startDate, endDate).then(async res => {
      const returnedTours = await res.json();
      setTours(returnedTours);
    });

  return (
    <ReportLayout>
      <Card width="500px">
        <Header type="h4">
          Lookup ClassTours
          <Link to="create">
            <Button align="right" intent="secondary">
              Add Class Tour
            </Button>
          </Link>
        </Header>
        <Formik
          initialValues={{ startDate: '', endDate: '' }}
          onSubmit={(values, { setSubmitting }) => {
            loadTours().then(() => {
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
                label="Start Date"
              />
              <Field
                id="endDate"
                type="date"
                name="endDate"
                component={Input}
                label="End Date"
              />
              <Button type="Submit" align="right" disabled={isSubmitting}>
                Lookup
              </Button>
            </Form>
          )}
        </Formik>
      </Card>
      {tours && (
        <>
          <Table>
            <caption>
              <Header>
                Class Tours -{' '}
                <CSVLink data={tours} filename="classTourLookup">
                  Download
                </CSVLink>
              </Header>
            </caption>
            <thead align="left">
              <tr>
                <th>Name</th>
                <th>Total Tourists</th>
                <th>Date</th>
                <th align="center">Actions</th>
              </tr>
            </thead>
            <tbody>
              {tours.map(tour => (
                <ClassTourRow
                  key={tour.id}
                  tour={tour}
                  afterDelete={loadTours}
                />
              ))}
            </tbody>
          </Table>
        </>
      )}
    </ReportLayout>
  );
};

const ClassTourRow = ({
  tour: { id, name, numberOfStudents, dayVisited },
  afterDelete
}) => (
  <tr>
    <td>{name}</td>
    <td align="center">{numberOfStudents}</td>
    <td>{new Date(dayVisited).toLocaleString()}</td>
    <td align="right">
      <Link to={`update/${id}`}>
        <Button
          display="inline-block"
          intent="secondary"
          style={{ margin: '0 1rem' }}
        >
          Update
        </Button>
      </Link>
      <Button
        display="inline-block"
        intent="danger"
        style={{ margin: '0 1rem' }}
        onClick={() => {
          const goDelete = window.confirm(
            `You sure you want to delete ${name} record`
          );
          if (goDelete) deleteTour(id).then(afterDelete);
        }}
      >
        Delete
      </Button>
    </td>
  </tr>
);

export default ClassTourLookup;
