import React from 'react';
import type { Node } from 'react';
import { Formik, Form, Field } from 'formik';
import { Card, Header, Button, Input } from '../ui';
import StartToEndDateSchema from '../schemas/StartToEndDateSchema';

type Props = {
  onSubmit: () => any,
  name: string,
  submitText: string,
  startDate: string,
  endDate: string,
  children: Node
};

const StartToEndDateForm = ({
  onSubmit,
  name,
  submitText,
  startDate = '',
  endDate = '',
  children,
  ...props
}: Props) => {
  return (
    <Card>
      <Header>{name}</Header>
      <p>Enter begin and end date to query by</p>
      <Formik
        onSubmit={onSubmit}
        validationSchema={StartToEndDateSchema}
        initialValues={{ startDate, endDate }}
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
