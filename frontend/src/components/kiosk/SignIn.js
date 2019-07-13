import React, { useState } from 'react';
import styled from 'styled-components';
import { Link, navigate } from '@reach/router';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { Card, Input, Header, Button, FieldGroup, Checkbox } from '../../ui';
import useQuery from '../../hooks/useQuery';
import callApi from '../../utils/callApi';

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

const Reasons = [
  { name: 'Computer Use', id: 1 },
  { name: 'Bone Use', id: 2 },
  { name: 'Lab Use', id: 3 },
  { name: 'Misc', id: 4 }
];

const getReasons = () => {
  return new Promise(resolve => {
    setTimeout(() => {
      resolve(Reasons);
    }, 1000);
  });
};

const isWVUPEmail = email => email.match(/^[A-Z0-9._%+-]+@wvup.edu$/i);

// test email = mtmqbude26@wvup.edu
const SignIn = () => {
  const [reasons] = useQuery(getReasons);
  const [student, setStudent] = useState();

  const loadClassList = email => {
    getStudentInfoWithEmail(email)
      .then(async res => {
        const studentInfo = await res.json();
        console.log(studentInfo);
        setStudent(studentInfo);
      })
      .catch(e => alert(e.message));
  };

  return (
    <FullScreenContainer>
      <Card>
        <Link to="/">Go Back</Link>
        <Formik
          initialValues={{
            email: '',
            reasons: [],
            tutoring: false,
            courses: []
          }}
          validationSchema={SignInSchema}
          onSubmit={async (values, { setSubmitting }) => {
            const signIn = {
              ...values,
              personId: student.studentID,
              semesterId: student.semesterId,
              courses: values.courses.map(courseCRN => {
                return student.classSchedule.find(ele => ele.crn === courseCRN);
              }),
              reasons: null
            };
            postSignIn(signIn)
              .then(async res => {
                if (res.status === 201) {
                  // navigate to home page
                  alert('You are signed in! ');
                  navigate('/');
                }
              })
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
  console.log(courses);
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
