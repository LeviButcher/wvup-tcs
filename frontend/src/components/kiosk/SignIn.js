import React from 'react';
import styled from 'styled-components';
import { Formik, Form, Field } from 'formik';
import Card from '../../ui/Card';
import Input from '../../ui/Input';

const SignIn = () => (
  <FullScreenContainer>
    <StyledCard>
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
            <header style={{ 'font-size': '25px' }}>Sign In</header>
            <Field type="email" name="email" component={Input} label="Email" />
            <button type="submit" disabled={isSubmitting}>
              Submit
            </button>
          </Form>
        )}
      </Formik>
    </StyledCard>
  </FullScreenContainer>
);

const StyledCard = styled(Card)`
  width: fit-content;
  margin: auto;
  padding: 4rem;
  box-shadow: 0 0 5px 1px;
`;

const FullScreenContainer = styled.div`
  height: calc(100vh - 75px);
  display: flex;
  align-items: center;
  justify-content: space-evenly;
`;

export default SignIn;
