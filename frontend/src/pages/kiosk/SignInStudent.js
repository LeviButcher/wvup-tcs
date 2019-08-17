import React from 'react';
import { Router, navigate } from '@reach/router';
import styled from 'styled-components';
import ScaleLoader from 'react-spinners/ScaleLoader';
import { Formik, Form, Field } from 'formik';
import { pipe } from 'ramda';
import { Card, Input, Button } from '../../ui';
import { callApi, unwrapToJSON, ensureResponseCode } from '../../utils';
import CoursesCheckboxes from '../../components/CoursesCheckboxes';
import ReasonCheckboxes from '../../components/ReasonCheckboxes';
import LoadingContent from '../../components/LoadingContent';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';
import SignInSchema from '../../schemas/SignInFormScema';

const postSignIn = callApi(`signins/`, 'POST');

const getStudentInfoWithEmail = email =>
  callApi(`signins/${email}/email`, 'GET', null);

const isWVUPEmail = email => email.match(/^[A-Z0-9._%+-]+@wvup.edu$/i);

// test email = mtmqbude26@wvup.edu
const SignInStudent = ({ navigate }) => {
  return (
    <FullScreenContainer>
      <Card>
        <h1>Student SignIn</h1>
        <Router primary={false}>
          <EmailOrCardSwipeForm
            default
            afterValidSubmit={studentInfo => {
              navigate('valid', { state: { studentInfo } });
            }}
          />
          <StudentSignInForm path="valid" />
        </Router>
      </Card>
    </FullScreenContainer>
  );
};

// email or listen for card swipe, check banner on submit, go to next
// swipe email make auto submit
// show errors
// add validation schema
const EmailOrCardSwipeForm = ({ afterValidSubmit }) => {
  return (
    <Formik
      onSubmit={({ email, id }, { setSubmitting }) => {
        getStudentInfoWithEmail(email)
          .then(ensureResponseCode(200))
          .then(unwrapToJSON)
          .then(afterValidSubmit)
          .finally(() => setSubmitting(false));
      }}
      initialValues={{ email: '', id: '' }}
    >
      {({ isSubmitting, isValid }) => (
        <Form>
          <h4>Please enter email or swipe card</h4>
          <Field
            id="email"
            type="text"
            name="email"
            component={Input}
            label="Email"
          />
          {isSubmitting && <h5>Getting Student information...</h5>}
          <ScaleLoader
            sizeUnit="px"
            size={150}
            loading={isSubmitting}
            align="center"
          />
          {!isSubmitting && (
            <Button type="Submit" disabled={!isValid} align="right">
              Submit
            </Button>
          )}
        </Form>
      )}
    </Formik>
  );
};

// id is valid student id, load reasons and courses for student
// still need to handle invalid id
const StudentSignInForm = ({
  location: {
    state: { studentInfo }
  }
}) => {
  const [loading, { body: reasons, headers }, errors] = useApiWithHeaders(
    'reasons/active'
  );
  console.log(studentInfo);
  return (
    <>
      <h4>Welcome, {`${studentInfo.firstName} ${studentInfo.lastName}`}</h4>
      <p>Please select a reason for vising the center and course</p>
      <Formik
        onSubmit={(studentSignIn, { setSubmitting }) => {
          const signIn = {
            ...studentSignIn,
            courses: studentSignIn.courses.map(courseCRN =>
              studentInfo.classSchedule.find(ele => ele.crn === courseCRN)
            ),
            reasons: studentSignIn.reasons.map(id =>
              reasons.find(ele => ele.id === id)
            )
          };
          postSignIn(signIn)
            .then(ensureResponseCode(201))
            .then(() => navigate('/'))
            .catch(e => alert(e.message))
            .finally(() => setSubmitting(false));
        }}
        initialValues={{
          email: studentInfo.studentEmail,
          reasons: [],
          tutoring: false,
          courses: [],
          semesterId: studentInfo.semesterId,
          personId: studentInfo.studentID
        }}
        validationSchema={SignInSchema}
      >
        {({ values, isSubmitting, isValid }) => (
          <Form>
            <LoadingContent
              loading={loading}
              data={{ headers, body: reasons }}
              errors={errors}
            >
              <ReasonCheckboxes reasons={reasons} values={values} />
            </LoadingContent>
            <CoursesCheckboxes courses={studentInfo.classSchedule} />
            {isSubmitting && <h5>Submitting SignIn...</h5>}
            <ScaleLoader sizeUnit="px" size={150} loading={isSubmitting} />
            {!isSubmitting && (
              <Button type="Submit" align="right" disabled={!isValid}>
                Submit
              </Button>
            )}
          </Form>
        )}
      </Formik>
    </>
  );
};

const FullScreenContainer = styled.div`
  padding: ${props => props.theme.padding};
  height: calc(100vh - 75px);
  display: flex;
  align-items: center;
  justify-content: space-evenly;
`;

export default SignInStudent;
