import React from 'react';
import { Form, Field, Formik } from 'formik';
import { Input, Button, FieldGroup, Table, Header } from '../../ui';

const Tours = [
  { name: 'Parkersburg South', count: '24', date: Date.now() },
  { name: 'Parkersburg North', count: '24', date: Date.now() },
  { name: 'Ripley', count: '24', date: Date.now() },
  { name: 'Ravenswood', count: '24', date: Date.now() }
];

const ClassTourLookup = () => {
  return (
    <div>
      <Formik>
        {() => (
          <Form>
            <FieldGroup>
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
              <Button>Lookup</Button>
            </FieldGroup>
          </Form>
        )}
      </Formik>
      <hr />
      <br />
      <br />
      <Table>
        <caption>
          <Header>Class Tour</Header>
        </caption>
        <thead align="left">
          <tr>
            <th>Name</th>
            <th>Total Count</th>
            <th>Date</th>
          </tr>
        </thead>
        <tbody>
          {Tours.map(tour => (
            <tr>
              <td>{tour.name}</td>
              <td>{tour.count}</td>
              <td>{tour.date}</td>
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
  );
};

export default ClassTourLookup;
