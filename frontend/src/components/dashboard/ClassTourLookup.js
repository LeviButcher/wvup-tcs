import React from 'react';
import { Link } from '@reach/router';
import { Form, Field, Formik } from 'formik';
import { Input, Button, FieldGroup, Table, Header } from '../../ui';

const Tours = [
  { id: 5, name: 'Parkersburg South', count: '24', date: Date.now() },
  { id: 6, name: 'Parkersburg North', count: '24', date: Date.now() },
  { id: 7, name: 'Ripley', count: '24', date: Date.now() },
  { id: 4, name: 'Ravenswood', count: '24', date: Date.now() }
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
      <Header align="right" type="h4">
        <Link to="create">
          <Button display="inline">Add Class Tour</Button>
        </Link>
      </Header>

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
            <ClassTourRow key={tour.id} tour={tour} />
          ))}
        </tbody>
      </Table>
    </div>
  );
};

const ClassTourRow = ({ tour: { id, name, count, date } }) => (
  <tr>
    <td>{name}</td>
    <td>{count}</td>
    <td>{date}</td>
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
