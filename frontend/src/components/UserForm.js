/* eslint-disable no-alert */
import React from 'react';
import { Form, Field, Formik } from 'formik';
import { navigate } from '@reach/router';
import * as Yup from 'yup';
import { Input, Button, Header, Card, Stack } from '../ui';
import { callApi, ensureResponseCode } from '../utils';
import type { User } from '../types';

const isCreate = (val): boolean => val === 'Create';
const UserSchema = Yup.object().shape({
  username: Yup.string()
    .trim()
    .required('Username is required'),
  // Password does not need to be required when creating a new user
  // Don't want to force people to overwrite their password
  password: Yup.string().when('action', {
    // $FlowFixMe
    is: isCreate,
    then: Yup.string().required('Password is required'),
    otherwise: Yup.string()
  }),
  action: Yup.string(),
  firstName: Yup.string().trim(),
  lastName: Yup.string().trim()
});

const postUser = callApi(`users/register`, 'POST');
const putUser = user => callApi(`users/${user.id}`, 'PUT', user);

const createUser = user =>
  postUser(user)
    .then(ensureResponseCode(201))
    .then(() => {
      alert(`Created user for ${user.username}`);
      navigate('/dashboard/admin/users');
    });

const updateUser = user =>
  putUser(user)
    .then(ensureResponseCode(200))
    .then(() => {
      alert(`Updated user for ${user.username}`);
      navigate('/dashboard/admin/users');
    });

const userDefault = {
  username: '',
  firstName: '',
  lastName: ''
};

type Props = {
  user?: User
};

const UserForm = ({ user = userDefault }: Props) => {
  const action = user === userDefault ? 'Create' : 'Update';

  const callCorrectApiFunc = values => {
    if (action === 'Create') {
      return createUser(values);
    }
    return updateUser(values);
  };

  return (
    <Card style={{ margin: 'auto' }}>
      <Formik
        validationSchema={UserSchema}
        initialValues={{ ...user, action, password: '' }}
        onSubmit={(
          { firstName, lastName, password, username, id },
          { setStatus }
        ) => {
          const submitUser = { username, firstName, lastName, password, id };
          return callCorrectApiFunc(submitUser).catch(e => {
            setStatus(e.message);
          });
        }}
        isInitialValid={false}
      >
        {({ status, isSubmitting, isValid }) => (
          <Form>
            <Stack>
              <Header>{action} User</Header>
              {status && <div style={{ color: 'red' }}>{status}</div>}
              <div style={{ display: 'none' }}>
                <Field
                  id="action"
                  type="text"
                  component={Input}
                  name="action"
                />
                <Field id="id" type="text" component={Input} name="id" />
              </div>
              <Field
                id="username"
                type="text"
                name="username"
                component={Input}
                label="Username"
              />
              <Field
                id="password"
                type="password"
                name="password"
                component={Input}
                label="Password"
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
              <Button
                type="submit"
                fullWidth
                disabled={isSubmitting || !isValid}
              >
                {isSubmitting ? 'Submitting...' : 'Submit'}
              </Button>
            </Stack>
          </Form>
        )}
      </Formik>
    </Card>
  );
};

UserForm.defaultProps = {
  user: userDefault
};

export default UserForm;
