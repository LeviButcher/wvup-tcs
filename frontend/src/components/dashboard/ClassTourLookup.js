import React, { useState } from 'react';
import styled from 'styled-components';
import { Link } from '@reach/router';
import { Form, Field, Formik } from 'formik';
import { Input, Button, Card, Table, Header } from '../../ui';
import callApi from '../../utils/callApi';

const getClassTours = callApi(
  `${process.env.REACT_APP_BACKEND}classtours/`,
  'GET'
);

const ClassTourLookup = () => {
  const [tours, setTours] = useState();
  return (
    <LookupLayout>
      <Card width="500px">
        <Header type="h4">
          Lookup ClassTours
          <Button align="right" intent="secondary">
            <Link to="create"> Add Class Tour</Link>
          </Button>
        </Header>
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

              <Button type="Submit" align="right">
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
              <Header>Class Tours</Header>
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
                <ClassTourRow key={tour.id} tour={tour} />
              ))}
            </tbody>
          </Table>
        </>
      )}
    </LookupLayout>
  );
};

const ClassTourRow = ({ tour: { id, name, numberOfStudents, dayVisited } }) => (
  <tr>
    <td>{name}</td>
    <td align="center">{numberOfStudents}</td>
    <td>{new Date(dayVisited).toLocaleString()}</td>
    <td align="right">
      <Button
        display="inline-block"
        intent="secondary"
        style={{ margin: '0 1rem' }}
      >
        <Link to={`update/${id}`}>Update</Link>
      </Button>
      <Button
        display="inline-block"
        intent="danger"
        style={{ margin: '0 1rem' }}
      >
        Delete
      </Button>
    </td>
  </tr>
);

const LookupLayout = styled.div`
  display: grid;
  grid-template: 'lookup table' 1fr / auto 1fr;
  grid-gap: 30px;
  & > * {
    align-self: flex-start;
    justify-self: center;
  }
`;

export default ClassTourLookup;
