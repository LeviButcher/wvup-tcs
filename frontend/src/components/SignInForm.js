import React from 'react';
import styled from 'styled-components';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Card, Input, Header, Button, Stack } from '../ui';
import { callApi, ensureResponseCode } from '../utils';
import CoursesCheckboxes from './CoursesCheckboxes';
import ReasonCheckboxes from './ReasonCheckboxes';
import SignInSchema from '../schemas/SignInFormSchema';
import type { Course, Reason } from '../types';

function isBeforeInTime(outTime) {
  const inTime = this.resolve(Yup.ref('inTime'));
  return inTime < outTime;
}

const TeacherSignInSchema = Yup.object().shape({
  inTime: Yup.string().required(),
  outTime: Yup.string()
    .required()
    .test('date-test', 'Must be after In Time', isBeforeInTime),
  inDate: Yup.date().required(),
  outDate: Yup.date().required()
});

const StudentSignInSchema = SignInSchema.shape({
  inTime: Yup.string()
    .typeError('Invalid Date format')
    .required(),
  outTime: Yup.string()
    .typeError('Invalid Date format')
    .test('date-test', 'Must be after In Time', isBeforeInTime)
    .required(),
  inDate: Yup.date().required(),
  outDate: Yup.date().required()
})
  .from('inTime', 'startDate', true)
  .from('outTime', 'endDate', true);

const FullScreenContainer = styled.div`
  padding: ${props => props.theme.padding};
  height: calc(100vh - 75px);
  display: flex;
  align-items: center;
  justify-content: space-evenly;
`;

const StyledReasonCheckboxes = styled(ReasonCheckboxes)`
  & label[data-deleted='true'] {
    color: red;
    border-color: red;
  }
  & label[data-deleted='true'][data-checked='true'] {
    color: white;
    background: red;
  }
  & label[data-deleted='false'] {
    color: green;
    border-color: green;
  }
  & label[data-deleted='false'][data-checked='true'] {
    color: white;
    background: green;
  }
`;

// Student API Calls
const putSignInStudent = signIn =>
  callApi(`signIns/${signIn.id || ''}`, 'PUT', signIn).then(
    ensureResponseCode(200)
  );
const postSignInStudent = signIn =>
  callApi(`signIns/admin/`, 'POST', signIn).then(ensureResponseCode(201));

// Teacher API Calls
const putSignInTeacher = signIn =>
  callApi(`signIns/${signIn.id || ''}?teacher=true`, 'PUT', signIn).then(
    ensureResponseCode(200)
  );
const postSignInTeacher = signIn =>
  callApi(`signIns/admin/?teacher=true`, 'POST', signIn).then(
    ensureResponseCode(201)
  );

type Props = {
  signInRecord: {
    email: string,
    classSchedule?: Array<Course>,
    selectedClasses: Array<string>,
    inTime: string,
    outTime: string,
    tutoring: boolean,
    selectedReasons: Array<string>,
    personType: 'Student' | 'Teacher',
    id?: string,
    personId: string,
    semesterId: string
  },
  reasons?: Array<Reason>
};

// test email = mtmqbude26@wvup.edu
const SignInForm = ({ signInRecord, reasons }: Props) => {
  const isStudent = signInRecord.personType === 'Student';

  const callCorrectAPIEndpoint = signIn => {
    // signInRecord id is NOT a truthy value then we should create a new signin record
    // ELSE we should update the signin record with the associated ID
    const shouldPostSignIn = !signInRecord.id;
    if (isStudent && shouldPostSignIn) return postSignInStudent(signIn);
    if (isStudent && !shouldPostSignIn) return putSignInStudent(signIn);
    if (!isStudent && shouldPostSignIn) return postSignInTeacher(signIn);
    if (!isStudent && !shouldPostSignIn) return putSignInTeacher(signIn);
    return Promise.reject(Error("Didn't hit a api case"));
  };

  return (
    <FullScreenContainer>
      <Card>
        <Formik
          initialValues={{
            email: signInRecord.email,
            reasons: signInRecord.selectedReasons,
            tutoring: signInRecord.tutoring,
            courses: signInRecord.selectedClasses,
            inTime: new Date(signInRecord.inTime).toTimeString(),
            outTime: new Date(signInRecord.outTime).toTimeString(),
            inDate: new Date(signInRecord.outTime).toDateString(),
            outDate: new Date(signInRecord.outTime).toDateString()
          }}
          isInitialValid={false}
          validationSchema={
            isStudent ? StudentSignInSchema : TeacherSignInSchema
          }
          onSubmit={(values, { setStatus }) => {
            // massage data back into server format
            const signIn = {
              id: signInRecord.id,
              personId: signInRecord.personId,
              semesterId: signInRecord.semesterId,
              inTime: new Date(`${values.inDate} ${values.inTime}`),
              outTime: new Date(`${values.outDate} ${values.outTime}`),
              courses: values.courses,
              reasons: values.reasons,
              tutoring: values.tutoring
            };
            // $FlowFixMe
            return callCorrectAPIEndpoint(signIn)
              .then(() => {
                alert('Success!');
                window.history.back();
              })
              .catch(e => setStatus(e.message));
          }}
        >
          {({ values, isSubmitting, status, isValid, errors, touched }) => (
            <Form>
              <Stack>
                <Header>Student Sign In</Header>
                <div>KEY: Green = Active, Red = Deleted</div>
                {status && <div>{status}</div>}
                <Field
                  id="email"
                  type="email"
                  name="email"
                  component={Input}
                  label="Email"
                />
                <Field
                  id="inDate"
                  type="date"
                  name="inDate"
                  component={Input}
                  label="In Date"
                />
                <Field
                  id="inTime"
                  type="time"
                  name="inTime"
                  component={Input}
                  label="In Time"
                />
                <Field
                  id="outDate"
                  type="date"
                  name="outDate"
                  component={Input}
                  label="Out Date"
                />
                <Field
                  id="outTime"
                  type="time"
                  name="outTime"
                  component={Input}
                  label="Out Time"
                />
                {isStudent && (
                  <>
                    <StyledReasonCheckboxes
                      reasons={reasons}
                      values={values}
                      errors={errors}
                      touched={touched}
                    />
                    <CoursesCheckboxes
                      courses={signInRecord.classSchedule || []}
                      errors={errors}
                      touched={touched}
                    />
                  </>
                )}
                <Button
                  type="submit"
                  disabled={isSubmitting || !isValid}
                  intent="primary"
                  fullWidth
                >
                  {isSubmitting ? 'Submitting...' : 'Submit'}
                </Button>
              </Stack>
            </Form>
          )}
        </Formik>
      </Card>
    </FullScreenContainer>
  );
};

SignInForm.defaultProps = {
  reasons: []
};

export default SignInForm;
