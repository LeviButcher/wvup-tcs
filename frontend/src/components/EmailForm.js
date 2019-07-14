import React from 'react';
import { Link } from '@reach/router';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Card, Input, Header, Button } from '../ui';

const emailSchema = Yup.object().shape({
  email: Yup.string()
    .email('Invalid email')
    .matches(/^[A-Z0-9._%+-]+@wvup.edu$/i, 'Must be a wvup email address')
    .trim()
    .required('Email is required')
});

const EmailForm = ({ title, onSubmit }) => (
  <Card>
    <Link to="/">Go Back</Link>
    <Formik
      initialValues={{ email: '' }}
      validationSchema={emailSchema}
      onSubmit={onSubmit}
    >
      {({ isSubmitting, isValid, status }) => (
        <Form>
          <Header>{title}</Header>
          {status && status.msg && (
            <div style={{ color: 'red' }}>{status.msg}</div>
          )}
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
            disabled={isSubmitting || !isValid}
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
