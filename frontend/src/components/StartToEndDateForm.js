import React from 'react';
import { Formik, Form, Field } from 'formik';
import { Card, Header, Button, Input } from '../ui';

const StartToEndDateForm = ({ onSubmit, name }) => {
  return (
    <Card>
      <Header>{name} Report</Header>
      <p>Enter begin and end date to query by</p>
      <Formik onSubmit={onSubmit}>
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
  );
};

export default StartToEndDateForm;
