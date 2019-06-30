import React from 'react';
import { Link } from '@reach/router';
import { Formik, Form, Field } from 'formik';
import { Card, Input, Header, Button } from '../ui';

const EmailForm = ({ title, onSubmit }) => (
  <Card>
    <Link to="/">Go Back</Link>
    <Formik
      initialValues={{ email: '' }}
      validate={values => {
        const errors = {};
        if (!values.email) {
          errors.email = 'Required';
        } else if (
          !/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(values.email)
        ) {
          errors.email = 'Invalid email address';
        }
        return errors;
      }}
      onSubmit={onSubmit}
    >
      {({ isSubmitting }) => (
        <Form>
          <Header>{title}</Header>
          <Field
            id="email"
            type="email"
            name="email"
            component={Input}
            label="Email"
          />
          <Button
            type="submit"
            align="right"
            disabled={isSubmitting}
            intent="primary"
          >
            Submit
          </Button>
        </Form>
      )}
    </Formik>
  </Card>
);

export default EmailForm;
