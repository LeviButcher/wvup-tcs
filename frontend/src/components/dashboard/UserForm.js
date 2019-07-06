import React from 'react';
import { Form, Field, Formik } from 'formik';
import { navigate } from '@reach/router';
import { Input, Button, Header, Card } from '../../ui';
import callApi from '../../utils/callApi';

const postUser = callApi(
  `${process.env.REACT_APP_BACKEND}users/register`,
  'POST'
);

function createUser(values, { setSubmitting, setStatus }) {
  console.log(values);
  const message = {};
  postUser(values)
    .then(async res => {
      if (res.status !== 201) {
        throw await res.json();
      }
      alert(`Created user for ${values.username}`);
      navigate('/dashboard/admin/users');
    })
    .catch(e => {
      message.msg = e.message;
    })
    .finally(() => {
      setSubmitting(false);
      setStatus(message);
    });
}

function updateClassTour(values, { setSubmitting, setStatus }) {
  const message = {};
  callApi(`${process.env.REACT_APP_BACKEND}users/${values.id}`, 'PUT', values)
    .then(async res => {
      if (res.status !== 200) {
        throw await res.json();
      }
      alert(`Updated user for ${values.username}`);
      navigate('/dashboard/admin/users');
    })
    .catch(e => {
      message.msg = e.message;
    })
    .finally(() => {
      setSubmitting(false);
      setStatus(message);
    });
}

function getSubmitForAction(action) {
  switch (action) {
    case 'Create':
      return createUser;
    case 'Update':
      return updateClassTour;
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
