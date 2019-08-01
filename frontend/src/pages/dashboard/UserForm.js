import React from 'react';
import { Form, Field, Formik } from 'formik';
import { navigate } from '@reach/router';
import { Input, Button, Header, Card } from '../../ui';
import { callApi, ensureResponseCode, errorToMessage } from '../../utils';

const postUser = callApi(`users/register`, 'POST');
const putUser = values => callApi(`users/${values.id}`, 'PUT', values);

function createUser(values, { setSubmitting, setStatus }) {
  postUser(values)
    .then(ensureResponseCode(201))
    .then(() => {
      alert(`Created user for ${values.username}`);
      navigate('/dashboard/admin/users');
    })
    .catch(errorToMessage)
    .then(setStatus)
    .finally(() => {
      setSubmitting(false);
    });
}

function updateUser(values, { setSubmitting, setStatus }) {
  putUser(values)
    .then(ensureResponseCode(200))
    .then(() => {
      alert(`Updated user for ${values.username}`);
      navigate('/dashboard/admin/users');
    })
    .catch(errorToMessage)
    .then(setStatus)
    .finally(() => {
      setSubmitting(false);
    });
}

function getSubmitForAction(action) {
  switch (action) {
    case 'Create':
      return createUser;
    case 'Update':
      return updateUser;
    default:
      return () =>
        console.error(
          'Never hit a case in transalating an action to a submittion function'
        );
  }
}

const userDefault = {
  username: '',
  firstName: '',
  lastName: ''
};

// do create and updates tours
const UserForm = ({ data = userDefault, action = 'Create' }) => {
  return (
    <Card style={{ margin: 'auto' }}>
      <Formik
        initialValues={{ ...data, password: '' }}
        onSubmit={getSubmitForAction(action)}
      >
        {({ status, isSubmitting }) => (
          <Form>
            <Header>{action} User</Header>
            {status && status.msg && (
              <div style={{ color: 'red' }}>{status.msg}</div>
            )}
            <Field
              id="username"
              type="text"
              name="username"
              component={Input}
              label="Username"
              required
            />
            <Field
              id="password"
              type="password"
              name="password"
              component={Input}
              label="Password"
              {...{ required: action === 'Create' }}
            />
            <Field
              id="firstName"
              type="text"
              name="firstName"
              component={Input}
              label="firstName"
            />
            <Field
              id="lastName"
              type="text"
              name="lastName"
              component={Input}
              label="lastName"
            />
            <Button align="right" disabled={isSubmitting} type="Submit">
              {action}
            </Button>
          </Form>
        )}
      </Formik>
    </Card>
  );
};

export default UserForm;
