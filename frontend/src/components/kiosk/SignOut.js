import React from 'react';
import styled from 'styled-components';
import { Formik, Form, Field } from 'formik';
import { Card, Input, Header, Button } from '../../ui';

const SignOut = () => (
  <FullScreenContainer>
    <Card>
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
        onSubmit={(values, { setSubmitting }) => {
          setTimeout(() => {
            setSubmitting(false);
          }, 1000);
        }}
      >
        {({ isSubmitting }) => (
          <Form>
            <Header>Student Sign Out</Header>
            <Field
              id="email"
              type="email"
              name="email"
              component={Input}
              label="Email"
            />
            <Button type="submit" align="right" disabled={isSubmitting}>
              Submit
            </Button>
          </Form>
        )}
      </Formik>
    </Card>
  </FullScreenContainer>
);

const FullScreenContainer = styled.div`
  padding: ${props => props.theme.padding};
  height: calc(100vh - ${props => props.theme.kioskHeaderSize});
  display: flex;
  align-items: flex-start;
  justify-content: space-evenly;
`;

export default SignOut;
