import React from 'react';
import { Router, Link } from '@reach/router';
import styled from 'styled-components';
import ScaleLoader from 'react-spinners/ScaleLoader';
import { Formik, Form } from 'formik';
import { Card, Button } from '../../ui';
import { callApi, ensureResponseCode } from '../../utils';
import CoursesCheckboxes from '../../components/CoursesCheckboxes';
import ReasonCheckboxes from '../../components/ReasonCheckboxes';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';
import SignInSchema from '../../schemas/SignInFormScema';
import EmailOrCardSwipeForm from '../../components/EmailOrCardSwipeForm';

const postSignIn = callApi(`signins/`, 'POST');

// test email = mtmqbude26@wvup.edu
const SignInStudent = ({ navigate }) => {
  return (
    <FullScreenContainer>
      <Card>
        <Link to="/">Go to Home Screen</Link>
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
  },
  navigate
}) => {
  const [loading, { body: reasons }] = useApiWithHeaders('reasons/active');
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
            .then(() =>
              navigate('/', { state: { info: 'You have signed in!' } })
            )
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
            {!isSubmitting && !loading && (
              <>
                <ReasonCheckboxes reasons={reasons} values={values} />
                <CoursesCheckboxes courses={studentInfo.classSchedule} />
                <Button type="Submit" align="right" disabled={!isValid}>
                  Submit
                </Button>
              </>
            )}
            <ScaleLoader
              sizeUnit="px"
              size={150}
              loading={isSubmitting || loading}
            />
            {isSubmitting && <h5>Submitting SignIn...</h5>}
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
