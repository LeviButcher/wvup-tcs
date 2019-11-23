import React from 'react';
import { Formik, Form } from 'formik';
import { navigate } from '@reach/router';
import { Button, Stack } from '../ui';
import { callApi, ensureResponseCode } from '../utils';
import CoursesCheckboxes from './CoursesCheckboxes';
import ReasonCheckboxes from './ReasonCheckboxes';
import useApiWithHeaders from '../hooks/useApiWithHeaders';
import SignInSchema from '../schemas/SignInFormSchema';
import type { Student } from '../types';

const postSignIn = callApi(`signins/`, 'POST');

type Props = {
  student: Student
};

const StudentSignInForm = ({ student }: Props) => {
  const [, { body: reasons }] = useApiWithHeaders('reasons/active');

  return (
    <Formik
      onSubmit={studentSignIn => {
        const signIn = {
          ...studentSignIn,
          courses: studentSignIn.courses.map(courseCRN =>
            student.classSchedule.find(ele => ele.crn === courseCRN)
          ),
          reasons: studentSignIn.reasons.map(id =>
            reasons.find(ele => ele.id === id)
          )
        };
        return postSignIn(signIn)
          .then(ensureResponseCode(201))
          .then(() => navigate('/', { state: { info: 'You have signed in!' } }))
          .catch(e => console.log(e.message));
      }}
      initialValues={{
        email: student.studentEmail,
        reasons: [],
        tutoring: false,
        courses: [],
        semesterId: student.semesterId,
        personId: student.studentID
      }}
      validationSchema={SignInSchema}
    >
      {({ values, isSubmitting, isValid, errors, touched }) => (
        <Form>
          <Stack>
            <h4>Welcome, {`${student.firstName} ${student.lastName}`}</h4>
            <ReasonCheckboxes
              reasons={reasons}
              values={values}
              errors={errors}
              touched={touched}
            />
            <CoursesCheckboxes
              courses={student.classSchedule}
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
