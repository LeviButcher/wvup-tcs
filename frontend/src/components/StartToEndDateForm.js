import React from 'react';
import { Formik, Form, Field } from 'formik';
import { Card, Header, Button, Input } from '../ui';
import StartToEndDateSchema from '../schemas/StartToEndDateSchema';

const StartToEndDateForm = ({
  onSubmit,
  name,
  submitText,
  startDate = '',
  endDate = '',
  children,
  ...props
}) => {
  return (
    <Card>
      <Header>{name}</Header>
      <p>Enter begin and end date to query by</p>
      <Formik
        onSubmit={onSubmit}
        validationSchema={StartToEndDateSchema}
        initialValues={{ startDate, endDate }}
        isInitialValid={e => {
          return StartToEndDateSchema.validate(e.initialValues)
            .then(() => true)
            .catch(() => false);
        }}
        {...props}
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
            {children}
            <Button
              type="submit"
              align="right"
              intent="primary"
              disabled={isSubmitting || !isValid}
            >
              {submitText || 'Run Report'}
            </Button>
          </Form>
        )}
      </Formik>
    </Card>
  );
};

export default StartToEndDateForm;
