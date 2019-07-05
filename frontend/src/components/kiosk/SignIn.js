import React, { useState } from 'react';
import styled from 'styled-components';
import { Link } from '@reach/router';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { Card, Input, Header, Button, FieldGroup, Checkbox } from '../../ui';
import useQuery from '../../hooks/useQuery';

const SignInSchema = Yup.object().shape({
  email: Yup.string()
    .email('Invalid email')
    .required('Email is required'),
  courses: Yup.array()
    .required('Please select at least one course')
    .min(1),
  reasons: Yup.array()
    .required('Please select at least one reason for visiting')
    .min(1),
  tutoring: Yup.boolean()
});

const Reasons = [
  { name: 'Computer Use', id: 1 },
  { name: 'Bone Use', id: 2 },
  { name: 'Lab Use', id: 3 },
  { name: 'Misc', id: 4 }
];

const Courses = [
  { courseName: 'CS121', CRN: '31345' },
  { courseName: 'EDUC101', CRN: '23456' },
  { courseName: 'GBUS304', CRN: '78924' },
  { courseName: 'SEC300', CRN: '65798' },
  { courseName: 'CS129', CRN: '32156' }
];

const getReasons = () => {
  return new Promise(resolve => {
    setTimeout(() => {
      resolve(Reasons);
    }, 1000);
  });
};

const getCourses = () => {
  return new Promise(resolve => {
    setTimeout(() => {
      resolve(Courses);
    }, 1000);
  });
};

const isWVUPAddress = email => /^[A-Z0-9._%+-]+@wvup.edu$/i.test(email);

const SignIn = () => {
  const [reasons] = useQuery(getReasons);
  const [courses, setCourses] = useState();

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
              const res = await getCourses();
              setCourses(res);
            }
            return errors;
          }}
          onSubmit={async (values, { setSubmitting }) => {
            setSubmitting(false);
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
              {courses && (
                <>
                  <CoursesCheckboxes courses={courses} />
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
      <label>
        Tutoring
        <Field
          id="tutoring"
          type="checkbox"
          name="tutoring"
          component="input"
          label="Tutoring"
          value="Tutoring"
        />
      </label>
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
          key={course.CRN}
          id={course.CRN}
          type="checkbox"
          name="courses"
          label={course.courseName}
          value={course.CRN}
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

export default SignIn;
