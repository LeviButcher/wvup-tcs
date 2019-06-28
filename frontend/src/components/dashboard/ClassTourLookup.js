import React, { useEffect, useState } from 'react';
import { Link } from '@reach/router';
import { Form, Field, Formik } from 'formik';
import { Input, Button, FieldGroup, Table, Header } from '../../ui';
import callApi from '../../utils/callApi';

const getClassTours = callApi(
  `${process.env.REACT_APP_BACKEND}classtours/`,
  'GET'
);

const ClassTourLookup = () => {
  const [tours, setTours] = useState();
  return (
    <div>
      <Formik
        initialValues={{ startDate: '', endDate: '' }}
        onSubmit={() => {
          getClassTours(null).then(async res => {
            const returnedTours = await res.json();
            setTours(returnedTours);
          });
        }}
      >
        {() => (
          <Form>
            <FieldGroup>
              <Field
                id="startDate"
                type="date"
                name="startDate"
                component={Input}
                label="Start Date"
                required
              />
              <Field
                id="endDate"
                type="date"
                name="endDate"
                component={Input}
                label="End Date"
                required
              />
              <Button type="Submit">Lookup</Button>
            </FieldGroup>
          </Form>
        )}
      </Formik>
      <hr />
      <Header align="right" type="h4">
        <Link to="create">
          <Button display="inline">Add Class Tour</Button>
        </Link>
      </Header>
      {tours && (
        <>
          <Table>
            <caption>
              <Header>Class Tours</Header>
            </caption>
            <thead align="left">
              <tr>
                <th>Name</th>
                <th>Total Tourists</th>
                <th>Date</th>
              </tr>
            </thead>
            <tbody>
              {tours.map(tour => (
                <ClassTourRow key={tour.id} tour={tour} />
              ))}
            </tbody>
          </Table>
        </>
      )}
    </div>
  );
};

const ClassTourRow = ({ tour: { id, name, numberOfStudents, dayVisited } }) => (
  <tr>
    <td>{name}</td>
    <td>{numberOfStudents}</td>
    <td>{new Date(dayVisited).toLocaleString()}</td>
    <td>
      <Button display="inline">
        <Link to={`update/${id}`}>Update</Link>
      </Button>
      <Button display="inline">
        <Link to={`delete/${id}`}>Delete</Link>
      </Button>
    </td>
  </tr>
);

export default ClassTourLookup;
