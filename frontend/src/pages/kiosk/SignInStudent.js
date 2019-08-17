import React from 'react';
import { Router, navigate } from '@reach/router';
import styled from 'styled-components';
import ScaleLoader from 'react-spinners/ScaleLoader';
import { Formik, Form } from 'formik';
import { Card, Button } from '../../ui';
import { callApi, ensureResponseCode } from '../../utils';
import CoursesCheckboxes from '../../components/CoursesCheckboxes';
import ReasonCheckboxes from '../../components/ReasonCheckboxes';
import LoadingContent from '../../components/LoadingContent';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';
import SignInSchema from '../../schemas/SignInFormScema';
import EmailOrCardSwipeForm from '../../components/EmailOrCardSwipeForm';

const postSignIn = callApi(`signins/`, 'POST');

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
