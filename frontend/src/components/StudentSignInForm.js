import React from 'react';
import { Formik, Form } from 'formik';
import { navigate } from '@reach/router';
import { Button, Stack } from '../ui';
import { apiFetch } from '../utils/fetchLight';
import CoursesCheckboxes from './CoursesCheckboxes';
import ReasonCheckboxes from './ReasonCheckboxes';
import useApi from '../hooks/useApi';
import SignInSchema from '../schemas/SignInFormSchema';
import type { Student } from '../types';

const postSignIn = signIn => apiFetch(`sessions/in`, 'POST', signIn);

type Props = {
  student: Student
};

const StudentSignInForm = ({ student }: Props) => {
  const [, reasons] = useApi('reasons/active');

  return (
    <Formik
      onSubmit={(studentSignIn, { setStatus }) => {
        const signIn = {
          personId: studentSignIn.personId,
          tutoring: studentSignIn.tutoring,
          selectedReasons: studentSignIn.reasons,
          selectedClasses: studentSignIn.courses
        };

        // $FlowFixMe
        return postSignIn(signIn)
          .then(() => {
            // $FlowFixMe
            navigate('/', {
              state: { info: 'You have signed in!' }
            });
          })
          .catch(e => e.unwrapFetchErrorMessage())
          .then(m => setStatus(m));
      }}
      initialValues={{
        email: student.email,
        reasons: [],
        tutoring: false,
        courses: [],
        personId: student.id
      }}
      validationSchema={SignInSchema}
    >
      {({ values, isSubmitting, isValid, errors, touched, status }) => (
        <Form>
          <Stack>
            <h4>Welcome, {`${student.firstName} ${student.lastName}`}</h4>
            <h5>{status}</h5>
            <ReasonCheckboxes
              reasons={reasons || []}
              values={values}
              errors={errors}
              touched={touched}
            />
            <CoursesCheckboxes
              courses={student.schedule}
              errors={errors}
              touched={touched}
            />
            <Button
              type="submit"
              disabled={!isValid || isSubmitting}
              fullWidth
              intent="primary"
            >
              {isSubmitting ? 'Submitting SignIn...' : 'Submit'}
            </Button>
          </Stack>
        </Form>
      )}
    </Formik>
  );
};

export default StudentSignInForm;
