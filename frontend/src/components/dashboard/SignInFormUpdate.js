import React, { useState } from 'react';
import styled from 'styled-components';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { pipe } from 'ramda';
import { Card, Input, Header, Button, FieldGroup, Checkbox } from '../../ui';
import useQuery from '../../hooks/useQuery';
import { callApi, unwrapToJSON, ensureResponseCode } from '../../utils';

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

const putSignIn = callApi(`signins/`, 'PUT');

const getStudentInfoWithEmail = email =>
  callApi(`signins/${email}/email`, 'GET', null);

const getReasons = () => callApi(`reasons`, 'GET', null);

const queryReasons = pipe(
  getReasons,
  ensureResponseCode(200),
  unwrapToJSON
);

const isWVUPEmail = email => email.match(/^[A-Z0-9._%+-]+@wvup.edu$/i);

// test email = mtmqbude26@wvup.edu
const SignIn = ({ afterSuccessfulSubmit, data }) => {
  const [reasons] = useQuery(queryReasons);
  const [student, setStudent] = useState(data);
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
            email: student.email,
            reasons: student.reasons.map(reason => reason.id),
            tutoring: student.tutoring,
            courses: student.courses.map(course => course.crn)
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
            putSignIn(signIn)
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
                disabled
              />
              {reasons && <ReasonsCheckboxes reasons={reasons} />}
              {student && (
                <>
                  <CoursesCheckboxes
                    courses={student.classSchedule || student.courses}
                  />
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
          type="checkbox"
          name="reasons"
          label={`${reason.name}`}
          value={reason.id}
          style={{
            color: reason.deleted ? 'red' : 'green'
          }}
          title={`This reason is ${reason.deleted ? 'deleted' : 'active'}`}
        />
      ))}
    </FieldGroup>
  </>
);

const CoursesCheckboxes = ({ courses }) => {
  return (
    <>
      <Header type="h4">
        Classes Visiting for <SmallText>Select at least one course</SmallText>
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
