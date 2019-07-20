import React, { useState } from 'react';
import styled from 'styled-components';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { pipe } from 'ramda';
import unWrapToJSON from '../../utils/unwrapToJSON';
import { Card, Input, Header, Button, FieldGroup, Checkbox } from '../../ui';
import useQuery from '../../hooks/useQuery';
import callApi from '../../utils/callApi';
import ensureResponseCode from '../../utils/ensureResponseCode';

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

const postSignIn = callApi(`${process.env.REACT_APP_BACKEND}signins/`, 'POST');

const getStudentInfoWithEmail = email =>
  callApi(
    `${process.env.REACT_APP_BACKEND}signins/${email}/email`,
    'GET',
    null
  );

const getReasons = () =>
  callApi(`${process.env.REACT_APP_BACKEND}reasons/active`, 'GET', null);

const queryReasons = pipe(
  getReasons,
  unWrapToJSON
);

const isWVUPEmail = email => email.match(/^[A-Z0-9._%+-]+@wvup.edu$/i);

// test email = mtmqbude26@wvup.edu
const SignIn = ({ afterSuccessfulSubmit }) => {
  const [reasons] = useQuery(queryReasons);
  const [student, setStudent] = useState();

  const loadClassList = email => {
    getStudentInfoWithEmail(email)
      .then(ensureResponseCode(200))
      .then(unWrapToJSON)
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
          {({ isSubmitting, status, isValid, handleChange }) => (
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
              {reasons && <ReasonsCheckboxes reasons={reasons} />}
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

const ReasonsCheckboxes = ({ reasons }) => (
  <>
    <Header type="h4">
      Reason for Visiting{' '}
      <SmallText>Select Tutoring or at least one other reason</SmallText>
      <ErrorMessage name="reasons">
        {message => <div style={{ color: 'red' }}>{message}</div>}
      </ErrorMessage>
      <ErrorMessage name="tutoring">
        {message => <div style={{ color: 'red' }}>{message}</div>}
      </ErrorMessage>
    </Header>
    <FieldGroup>
      <SingleCheckBoxLabel name="tutoring">
        Tutoring
        <Field
          id="tutoring"
          type="checkbox"
          name="tutoring"
          component="input"
          label="tutoring"
          value="Tutoring"
        />
      </SingleCheckBoxLabel>
      {reasons.map(reason => (
        <Checkbox
          key={reason.id}
          id={reason.name}
          name="reasons"
          label={reason.name}
          value={reason.id}
        />
      ))}
    </FieldGroup>
  </>
);

const CoursesCheckboxes = ({ courses }) => {
  return (
    <>
      <Header type="h4">
        Classes visiting for <SmallText>Select at least one course</SmallText>
        <ErrorMessage name="courses">
          {message => <div style={{ color: 'red' }}>{message}</div>}
        </ErrorMessage>
      </Header>
      <FieldGroup>
        {courses.map(course => (
          <Checkbox
            key={course.crn}
            id={course.crn}
            type="checkbox"
            name="courses"
            label={course.shortName}
            value={course.crn}
          />
        ))}
      </FieldGroup>
    </>
  );
};

const SmallText = styled.span`
  color: #aaa;
  font-size: 0.8em;
`;

const FullScreenContainer = styled.div`
  padding: ${props => props.theme.padding};
  height: calc(100vh - 75px);
  display: flex;
  align-items: center;
  justify-content: space-evenly;
`;

const SingleCheckBoxLabel = styled.label`
  display: flex;
  justify-content: space-between;
  align-items: center;
`;

export default SignIn;
