import React from 'react';
import styled from 'styled-components';
import { Form, Field, Formik } from 'formik';
import { navigate } from '@reach/router';
import callApi from '../../utils/callApi';
import { Card, Header, Input, Button } from '../../ui';

const postToAuthApi = callApi(
  `${process.env.REACT_APP_BACKEND}users/authenticate`,
  'POST'
);

const Login = () => {
  return (
    <CenterComponent>
      <Card>
        <Header align="center">Tutoring Success Center Login</Header>
        <Formik
          initialValues={{ username: '', password: '' }}
          onSubmit={async (values, { setSubmitting, setStatus }) => {
            const message = {};
            postToAuthApi(values)
              .then(async res => {
                if (res.status !== 200) {
                  throw await res.json();
                }
                // put token in localstorage
                const user = await res.json();
                localStorage.setItem(
                  `${process.env.REACT_APP_TOKEN}`,
                  user.token
                );
                navigate('/dashboard');
              })
              .catch(async error => {
                message.msg = error.message;
              })
              .finally(() => {
                setStatus(message);
                setSubmitting(false);
              });
          }}
        >
          {({ status, values, isSubmitting, handleSubmit }) => (
            <Form onSubmit={handleSubmit}>
              {status && status.msg && (
                <div style={{ color: 'red' }}>{status.msg}</div>
              )}
              <Field
                id="username"
                name="username"
                value={values.username}
                component={Input}
                label="Username"
              />
              <Field
                id="password"
                type="password"
                name="password"
                value={values.password}
                component={Input}
                label="Password"
              />
              <br />
              <Button
                type="submit"
                display="block"
                align="right"
                disabled={isSubmitting}
              >
                Submit
              </Button>
            </Form>
          )}
        </Formik>
      </Card>
    </CenterComponent>
  );
};

const CenterComponent = styled.div`
  height: 100vh;
  display: grid;
  align-items: center;
  justify-content: center;
`;

export default Login;
