import React from 'react';
import { Link } from '@reach/router';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Card, Input, Header, Button, Stack } from '../ui';

const emailSchema = Yup.object().shape({
  email: Yup.string()
    .email('Invalid email')
    .matches(/^[A-Z0-9._%+-]+@wvup.edu$/i, 'Must be a WVUP email address')
    .trim()
    .required('Email is required')
});

type Props = {
  title: string,
  onSubmit: () => any,
  errors: () => { message: string }
};

const EmailForm = ({ title, onSubmit, errors }: Props) => (
  <Card style={{ paddingBottom: 0 }}>
    <Link to="/">Go to Home Screen</Link>
    <Formik
      initialValues={{ email: '' }}
      validationSchema={emailSchema}
      onSubmit={onSubmit}
      isInitialValid={false}
    >
      {({ isSubmitting, isValid, status }) => (
        <Form>
          <Stack>
            <Header>{title}</Header>
            <h4>Please enter email or swipe card</h4>
            {status && status.msg && (
              <p style={{ color: 'red' }}>{status.msg}</p>
            )}
            {errors && <p style={{ color: 'red' }}>{errors.message}</p>}
            <Field
              id="email"
              type="email"
              name="email"
              component={Input}
              label="Email"
            />
            <Button
              type="submit"
              fullWidth
              disabled={isSubmitting || !isValid}
              intent="primary"
            >
              Submit
            </Button>
          </Stack>
        </Form>
      )}
    </Formik>
  </Card>
);

export default EmailForm;
