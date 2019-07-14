import React from 'react';
import { Formik, Form, Field } from 'formik';
import { Card, Header, Button, Input } from '../ui';

const StartToEndDateForm = ({ onSubmit, name, ...props }) => {
  return (
    <Card {...props}>
      <Header>{name} Report</Header>
      <p>Enter begin and end date to query by</p>
      <Formik
        onSubmit={onSubmit}
        initialValues={{ startDate: '', endDate: '' }}
      >
        {({ isSubmitting }) => (
          <Form>
            <Field
              id="startDate"
              type="date"
              name="startDate"
              component={Input}
              label="start Date"
              required
            />
            <Field
              id="endDate"
              type="date"
              name="endDate"
              component={Input}
              label="end Date"
              required
            />
            <Button
              type="submit"
              align="right"
              intent="primary"
              disabled={isSubmitting}
            >
              Run Report
            </Button>
          </Form>
        )}
      </Formik>
    </Card>
  );
};

export default StartToEndDateForm;
