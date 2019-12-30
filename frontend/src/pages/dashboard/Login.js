import React from 'react';
import styled from 'styled-components';
import { Form, Field, Formik } from 'formik';
import Topography from '../../images/topography.svg';
import { unwrapToJSON } from '../../utils';
import { apiFetch } from '../../utils/fetchLight';
import { Card, Header, Input, Button, Stack } from '../../ui';
import type { User } from '../../types';

const CenterComponent = styled.div`
  height: 100vh;
  display: grid;
  align-items: center;
  justify-content: center;
  background-color: #dfdbe5;
  background-image: url('${Topography}');
`;

const postToAuthApi = values => apiFetch(`users/authenticate`, 'POST', values);

const Login = () => {
  return (
    <CenterComponent>
      <Card>
        <Formik
          initialValues={{ username: '', password: '' }}
          isInitialValid={false}
          onSubmit={(values, { setStatus }) => {
            // $FlowFixMe
            return postToAuthApi(values)
              .then(unwrapToJSON)
              .then((user: User) => {
                // put user in local storage
                localStorage.setItem(
                  `${process.env.REACT_APP_TOKEN || ''}`,
                  user.token || ''
                );
                localStorage.setItem('username', user.username);
                window.history.back();
              })
              .catch(e => e.unwrapFetchErrorMessage())
              .then(message => setStatus(message));
          }}
        >
          {({ status, isSubmitting, isValid }) => (
            <Form>
              <Stack>
                <Header align="center">Tutoring Center Login</Header>
                {status && <div style={{ color: 'red' }}>{status}</div>}
                <Field
                  id="username"
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
                <Button
                  type="submit"
                  display="block"
                  fullWidth
                  disabled={isSubmitting || !isValid}
                >
                  Submit
                </Button>
              </Stack>
            </Form>
          )}
        </Formik>
      </Card>
    </CenterComponent>
  );
};

export default Login;
