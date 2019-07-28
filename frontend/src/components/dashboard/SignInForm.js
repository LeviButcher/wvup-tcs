import React, { useState } from 'react';
import styled from 'styled-components';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { pipe } from 'ramda';
import { Card, Input, Header, Button } from '../../ui';
import useQuery from '../../hooks/useQuery';
import { callApi, unwrapToJSON, ensureResponseCode } from '../../utils';
import CoursesCheckboxes from '../CoursesCheckboxes';
import ReasonCheckboxes from '../ReasonCheckboxes';

const SignInSchema = Yup.object().shape({
  email: Yup.string()
    .email('Invalid email')
    .matches(/^[A-Z0-9._%+-]+@wvup.edu$/i, 'Must be a wvup email address')
    .trim()
    .required('Email is required'),
  courses: Yup.array()
    .min(1)
    .required('A Course is required'),
  reasons: Yup.array().when('tutoring', {
    is: true,
    then: Yup.array(),
    otherwise: Yup.array()
      .min(1)
      .required()
  }),
  tutoring: Yup.boolean()
});

const postSignIn = callApi(`signins/`, 'POST');

const getStudentInfoWithEmail = email =>
  callApi(`signins/${email}/email`, 'GET', null);

const getReasons = () => callApi(`reasons/active`, 'GET', null);

const queryReasons = pipe(
  getReasons,
  unwrapToJSON
);

const isWVUPEmail = email => email.match(/^[A-Z0-9._%+-]+@wvup.edu$/i);

// test email = mtmqbude26@wvup.edu
const SignIn = ({ afterSuccessfulSubmit }) => {
  const [reasons] = useQuery(queryReasons);
  const [student, setStudent] = useState();

  const loadClassList = email => {
    getStudentInfoWithEmail(email)
      .then(ensureResponseCode(200))
      .then(unwrapToJSON)
      .then(setStudent)
      .catch(e => alert(e.message));
  };

  return (
    <FullScreenContainer>
      <Card>
        <Formik
          initialValues={{
            email: '',
            reasons: [],
            tutoring: false,
            courses: []
          }}
          validationSchema={SignInSchema}
          onSubmit={async (values, { setSubmitting }) => {
            // massage data back into server format
            const signIn = {
              ...values,
              personId: student.studentID,
              semesterId: student.semesterId,
              courses: values.courses.map(courseCRN =>
                student.classSchedule.find(ele => ele.crn === courseCRN)
              ),
              reasons: values.reasons.map(id =>
                reasons.find(ele => ele.id === id)
              )
            };
            postSignIn(signIn)
              .then(ensureResponseCode(201))
              .then(afterSuccessfulSubmit)
              .catch(e => alert(e.message))
              .finally(() => setSubmitting(false));
          }}
        >
          {({ values, isSubmitting, status, isValid, handleChange }) => (
            <Form>
              <Header>Student Sign In</Header>
              <p>Enter in Email to load classlist</p>
              {status && status.msg && <div>{status.msg}</div>}
              <Field
                id="email"
                type="email"
                name="email"
                component={Input}
                label="Email"
                onChange={e => {
                  handleChange(e);
                  if (isWVUPEmail(e.target.value))
                    loadClassList(e.target.value);
                }}
              />
              {reasons && (
                <ReasonCheckboxes reasons={reasons} values={values} />
              )}
              {student && (
                <>
                  <CoursesCheckboxes courses={student.classSchedule} />
                </>
              )}
              <br />
              <Button
                type="submit"
                align="right"
                disabled={isSubmitting || !isValid}
                intent="primary"
              >
                Submit
              </Button>
            </Form>
          )}
        </Formik>
      </Card>
    </FullScreenContainer>
  );
};

const FullScreenContainer = styled.div`
  padding: ${props => props.theme.padding};
  height: calc(100vh - 75px);
  display: flex;
  align-items: center;
  justify-content: space-evenly;
`;

export default SignIn;
