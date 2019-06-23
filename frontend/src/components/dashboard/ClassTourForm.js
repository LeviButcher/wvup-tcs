import React from 'react';
import { Form, Field, Formik } from 'formik';
import { navigate } from '@reach/router';
import { Input, Button, Header, Card } from '../../ui';

// pass in callback for submission, route to create or update
// add in hidden field for ID for update
// pass in ClassTour object fills in fields
//
// Lookup tour will grab tours data then pass down to component
//
// make default callback, creation function

function createClassTour() {
  setTimeout(() => {
    alert('Faked creation of tour');
    navigate('/dashboard/tours');
  }, 1000);
}

const classTourDefault = {
  name: '',
  date: '',
  count: ''
};

// do create and updates tours
const ClassTourForm = ({
  classTour = classTourDefault,
  onSubmit,
  action = 'Create'
}) => {
  return (
    <Card>
      <Formik onSubmit={onSubmit || createClassTour}>
        {({ isSubmitting }) => (
          <Form>
            <Header>{action} Tour</Header>
            <Field
              id="name"
              type="text"
              name="name"
              value={classTour.name}
              component={Input}
              label="Name"
            />
            <Field
              id="date"
              type="date"
              name="date"
              value={classTour.date}
              component={Input}
              label="Date Toured"
            />
            <Field
              id="count"
              type="number"
              name="count"
              value={classTour.count}
              component={Input}
              label="Number of Tourist"
            />
            <Button align="right" disabled={isSubmitting}>
              {action}
            </Button>
          </Form>
        )}
      </Formik>
    </Card>
  );
};

export default ClassTourForm;
