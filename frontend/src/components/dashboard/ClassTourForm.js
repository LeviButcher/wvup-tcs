import React from 'react';
import { Form, Field, Formik } from 'formik';
import { navigate } from '@reach/router';
import { Input, Button, Header, Card } from '../../ui';
import callApi from '../../utils/callApi';

const postClassTour = callApi(
  `${process.env.REACT_APP_BACKEND}classtours/`,
  'POST'
);

function createClassTour(values, { setSubmitting, setStatus }) {
  const message = {};
  postClassTour(values)
    .then(async res => {
      if (res.status !== 201) {
        throw await res.json();
      }
      alert(`Created tour for ${values.name}`);
      navigate('/dashboard/tours');
    })
    .catch(e => {
      message.msg = e.message;
    })
    .finally(() => {
      setSubmitting(false);
      setStatus(message);
    });
}

function updateClassTour(values, { setSubmitting, setStatus }) {
  const message = {};
  callApi(
    `${process.env.REACT_APP_BACKEND}classtours/${values.id}`,
    'PUT',
    values
  )
    .then(async res => {
      if (res.status !== 200) {
        throw await res.json();
      }
      alert(`Updated tour for ${values.name}`);
      navigate('/dashboard/tours');
    })
    .catch(e => {
      message.msg = e.message;
    })
    .finally(() => {
      setSubmitting(false);
      setStatus(message);
    });
}

function getSubmitForAction(action) {
  switch (action) {
    case 'Create':
      return createClassTour;
    case 'Update':
      return updateClassTour;
    default:
      return () =>
        console.error(
          'Never hit a case in transalating a action to a submittion function'
        );
  }
}

const classTourDefault = {
  name: '',
  dayVisited: '',
  numberOfStudents: ''
};

// do create and updates tours
const ClassTourForm = ({ classTour = classTourDefault, action = 'Create' }) => {
  return (
    <Card style={{ margin: 'auto' }}>
      <Formik initialValues={classTour} onSubmit={getSubmitForAction(action)}>
        {({ values, status, isSubmitting }) => (
          <Form>
            <Header>{action} Tour</Header>
            {status && status.msg && (
              <div style={{ color: 'red' }}>{status.msg}</div>
            )}
            <Field
              id="name"
              type="text"
              name="name"
              component={Input}
              label="Name"
            />
            <Field
              id="dayVisited"
              type="date"
              name="dayVisited"
              value={
                values.dayVisited.length > 0
                  ? new Date(values.dayVisited).toISOString().substr(0, 10)
                  : ''
              }
              component={Input}
              label="Date Toured"
            />
            <Field
              id="numberOfStudents"
              type="number"
              name="numberOfStudents"
              component={Input}
              label="Number of Tourist"
            />
            <Button align="right" disabled={isSubmitting} type="Submit">
              {action}
            </Button>
          </Form>
        )}
      </Formik>
    </Card>
  );
};

export default ClassTourForm;
