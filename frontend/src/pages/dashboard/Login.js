import React from 'react';
import styled from 'styled-components';
import { Form, Field, Formik } from 'formik';
import { navigate } from '@reach/router';
import Topography from '../../images/topography.svg';
import { callApi, ensureResponseCode, unwrapToJSON } from '../../utils';
import { Card, Header, Input, Button } from '../../ui';
import useLoading from '../../hooks/useLoading';

const postToAuthApi = callApi(`users/authenticate`, 'POST');

const Login = () => {
  const [loading, startLoading, doneLoading] = useLoading();

  return (
    <CenterComponent>
      <Card data-loading={loading}>
        <Header align="center">Tutoring Center Login</Header>
        <Formik
          initialValues={{ username: '', password: '' }}
          onSubmit={async (values, { setSubmitting, setStatus }) => {
            startLoading();
            postToAuthApi(values)
              .then(ensureResponseCode(200))
              .then(unwrapToJSON)
              .then(user => {
                // put user in local storage
                localStorage.setItem(
                  `${process.env.REACT_APP_TOKEN}`,
                  user.token
                );
                localStorage.setItem('username', user.username);
                navigate('/dashboard');
              })
              .catch(setStatus)
              .finally(() => {
                setSubmitting(false);
                doneLoading();
              });
          }}
        >
          {({ status, values, isSubmitting, handleSubmit }) => (
            <Form onSubmit={handleSubmit}>
              {status && status.message && (
                <div style={{ color: 'red' }}>{status.message}</div>
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
  background-color: #dfdbe5;
  background-image: url('${Topography}');
`;

export default Login;
