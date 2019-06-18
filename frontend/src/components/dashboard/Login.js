import React from 'react';
import styled from 'styled-components';
import { Form, Field, Formik } from 'formik';
import { Card, Header, Input, Button } from '../../ui';

const Login = () => {
  return (
    <CenterComponent>
      <Card>
        <Header align="center">Tutoring Success Center Login</Header>
        <Formik>
          {() => (
            <Form>
              <Field
                id="email"
                type="email"
                name="email"
                component={Input}
                label="Email"
              />
              <Field
                id="password"
                type="password"
                name="password"
                component={Input}
                label="Password"
              />
              <br />
              <Button display="block">Submit</Button>
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
