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
    .required('Email is required'),
  courses: Yup.array()
    .required('Please select at least one course')
    .min(1),
  reasons: Yup.array().min(1),
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

const isWVUPAddress = email => /^[A-Z0-9._%+-]+@wvup.edu$/i.test(email);

const findById = id => idProp => obj => obj[idProp] === id;

// test email = mtmqbude26@wvup.edu
const SignIn = () => {
  const [reasons] = useQuery(getReasons);
  const [student, setStudent] = useState();

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
          validate={async values => {
            const errors = {};
            if (!isWVUPAddress(values.email)) {
              errors.email = 'Must be wvup.edu email address';
            }
            if (!errors.email) {
              getStudentInfoWithEmail(values.email)
                .then(async res => {
                  const studentInfo = await res.json();
                  setStudent(studentInfo);
                })
                .catch(e => alert(e.message));
            }
            return errors;
          }}
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
                const bod = await res.json();
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
          {({ isSubmitting, status }) => (
            <Form>
              <Header>Student Sign In</Header>
              {status && status.msg && <div>{status.msg}</div>}
              <Field
                id="email"
                type="email"
                name="email"
                component={Input}
                label="Email"
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
                disabled={isSubmitting}
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
      Reason for Visiting
      <ErrorMessage name="reasons">
        {message => <div style={{ color: 'red' }}>{message}</div>}
      </ErrorMessage>
      <ErrorMessage name="tutoring">
        {message => <div style={{ color: 'red' }}>{message}</div>}
      </ErrorMessage>
    </Header>
    <FieldGroup>
      <SingleCheckBoxLabel>
        Tutoring
        <Field
          id="tutoring"
          type="checkbox"
          name="tutoring"
          component="input"
          label="Tutoring"
          value="Tutoring"
        />
      </SingleCheckBoxLabel>
      {reasons.map(reason => (
        <Checkbox
          key={reason.id}
          id={reason.id}
          name="reasons"
          label={reason.name}
          value={reason.id}
        />
      ))}
    </FieldGroup>
  </>
);

const CoursesCheckboxes = ({ courses }) => (
  <>
    <Header type="h4">
      Classes visiting for
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
          label={course.courseName}
          value={course.crn}
        />
      ))}
    </FieldGroup>
  </>
);

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
