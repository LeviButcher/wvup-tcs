import React from 'react';
import { Formik, Form, Field } from 'formik';
import { Card, Header, Button } from '../ui';

const StartToEndDateForm = ({ onSubmit, name, ...props }) => {
  return (
    <Card {...props}>
      <Header>{name} Report</Header>
      <p>Choose Semester to create a report for</p>
      <Formik onSubmit={onSubmit}>
        {({ isSubmitting }) => (
          <Form>
            <Field
              id="semester"
              name="semester"
              component="select"
              label="Semester"
            >
              <option>201901</option>
              <option>201902</option>
              <option>201903</option>
            </Field>
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
