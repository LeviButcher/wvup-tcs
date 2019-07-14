import React from 'react';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Card, Header, Button, Input } from '../ui';

const StartToEndDateSchema = Yup.object().shape({
  startDate: Yup.date().required(),
  endDate: Yup.date()
    .test('date-test', 'Must be after the Start Date', function(endDate) {
      return this.resolve(Yup.ref('startDate')) < endDate;
    })
    .required()
});

const StartToEndDateForm = ({ onSubmit, name, ...props }) => {
  return (
    <Card {...props}>
      <Header>{name} Report</Header>
      <p>Enter begin and end date to query by</p>
      <Formik
        onSubmit={onSubmit}
        validationSchema={StartToEndDateSchema}
        initialValues={{ startDate: '', endDate: '' }}
      >
        {({ isSubmitting, isValid }) => (
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
              disabled={isSubmitting || !isValid}
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
