import React from 'react';
import type { Node } from 'react';
import { Formik, Form, Field } from 'formik';
import { Card, Header, Button, Input, Stack } from '../ui';
import StartToEndDateSchema from '../schemas/StartToEndDateSchema';

type Props = {
  // Second argument is formikBag
  onSubmit: (any, any) => Promise<any> & any,
  title: string,
  initialValues?: {
    startDate: string,
    endDate: string
  },
  children?: Node
};

const StartToEndDateForm = ({ onSubmit, title, children, ...props }: Props) => {
  return (
    <Card>
      <Header>{title}</Header>
      <p>Enter begin and end date to query by</p>
      <Formik
        onSubmit={onSubmit}
        validationSchema={StartToEndDateSchema}
        enableReinitialize
        isInitialValid={false}
        {...props}
      >
        {({ isSubmitting, isValid }) => (
          <Form>
            <Stack>
              <Field
                id="startDate"
                type="date"
                name="startDate"
                component={Input}
                label="Start Date"
                required
              />
              <Field
                id="endDate"
                type="date"
                name="endDate"
                component={Input}
                label="End Date"
                required
              />
              {children}
              <Button
                type="submit"
                fullWidth
                intent="primary"
                disabled={isSubmitting || !isValid}
              >
                {isSubmitting ? 'Submitting' : 'Submit'}
              </Button>
            </Stack>
          </Form>
        )}
      </Formik>
    </Card>
  );
};

StartToEndDateForm.defaultProps = {
  initialValues: {
    startDate: '',
    endDate: ''
  },
  children: null
};

export default StartToEndDateForm;
