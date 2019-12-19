import React from 'react';
import styled from 'styled-components';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Input, Header, Button, Stack } from '../ui';
import { callApi, ensureResponseCode } from '../utils';
import CoursesCheckboxes from './CoursesCheckboxes';
import ReasonCheckboxes from './ReasonCheckboxes';
import SignInSchema from '../schemas/SignInFormSchema';
import type { Course, Reason, PersonType, Semester } from '../types';
import { personTypeValues } from '../types';
import SemesterDropdown from './SemesterDropdown';

function isBeforeInTime(outTime) {
  const inTime = this.resolve(Yup.ref('inTime'));
  return inTime < outTime;
}

const TeacherSignInSchema = Yup.object().shape({
  inTime: Yup.string().required(),
  outTime: Yup.string()
    .required()
    .test('date-test', 'Must be after In Time', isBeforeInTime),
  inDate: Yup.date()
    .typeError('Date is not in correct format')
    .required(),
  outDate: Yup.date()
    .typeError('Date is not in correct format')
    .required(),
  semesterCode: Yup.number().required()
});

const StudentSignInSchema = SignInSchema.shape({
  inTime: Yup.string()
    .typeError('Invalid Date format')
    .required(),
  outTime: Yup.string()
    .typeError('Invalid Date format')
    .test('date-test', 'Must be after In Time', isBeforeInTime)
    .required(),
  inDate: Yup.date()
    .typeError('Date is not in correct format')
    .required(),
  outDate: Yup.date()
    .typeError('Date is not in correct format')
    .required(),
  semesterCode: Yup.number().required()
})
  .from('inTime', 'startDate', true)
  .from('outTime', 'endDate', true);

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

const postSession = signIn =>
  callApi(`sessions/`, 'POST', signIn).then(ensureResponseCode(201));

const putSession = signIn =>
  callApi(`sessions/${signIn.id || ''}`, 'PUT', signIn).then(
    ensureResponseCode(200)
  );

const getDateOrTime = option => dateTimeString => {
  const [date, time] = dateTimeString.split(/[T]/);
  if (option === 'time') return time;
  return date;
};
const getDate = getDateOrTime('date');
const getTime = getDateOrTime('time');

type Props = {
  signInRecord: {
    email: string,
    schedule?: Array<Course>,
    selectedClasses: Array<string>,
    inTime: string,
    outTime: string,
    tutoring: boolean,
    selectedReasons: Array<string>,
    personType: PersonType,
    id?: string,
    personId: string,
    semesterCode?: number,
    firstName: string,
    lastName: string
  },
  semesters: Array<Semester>,
  reasons?: Array<Reason>
};

// test email = mtmqbude26@wvup.edu
const SignInForm = ({ signInRecord, reasons, semesters }: Props) => {
  const isStudent = signInRecord.personType === personTypeValues.student;
  const shouldPostSignIn = !signInRecord.id;

  const callCorrectAPIEndpoint = signIn => {
    // signInRecord id is NOT a truthy value then we should create a new signin record
    // ELSE we should update the signin record with the associated ID
    if (shouldPostSignIn) return postSession(signIn);
    if (!shouldPostSignIn) return putSession(signIn);

    return Promise.reject(Error("Didn't hit a api case"));
  };

  return (
    <Formik
      initialValues={{
        email: signInRecord.email,
        reasons: signInRecord.selectedReasons,
        tutoring: signInRecord.tutoring,
        courses: signInRecord.selectedClasses,
        inTime: getTime(signInRecord.inTime),
        outTime: getTime(signInRecord.outTime),
        inDate: getDate(signInRecord.inTime),
        outDate: getDate(signInRecord.outTime),
        semesterCode: signInRecord.semesterCode
      }}
      isInitialValid={false}
      validationSchema={isStudent ? StudentSignInSchema : TeacherSignInSchema}
      enableReinitialize
      onSubmit={(values, { setStatus }) => {
        // massage data back into server format
        const signIn = {
          id: signInRecord.id,
          personId: signInRecord.personId,
          semesterCode: values.semesterCode,
          inTime: `${values.inDate}T${values.inTime}`,
          outTime: `${values.outDate}T${values.outTime}`,
          selectedClasses: values.courses,
          selectedReasons: values.reasons,
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
            <Header>{shouldPostSignIn ? 'Create' : 'Update'} Sign In</Header>
            <div>KEY: Green = Active, Red = Deleted</div>
            {status && <div style={{ color: 'red' }}>{status}</div>}
            <h2>{`${signInRecord.firstName} ${signInRecord.lastName}`}</h2>
            <h3>{signInRecord.email}</h3>
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
            <SemesterDropdown semesters={semesters} />
            {isStudent && (
              <>
                <StyledReasonCheckboxes
                  reasons={reasons}
                  values={values}
                  errors={errors}
                  touched={touched}
                />
                <CoursesCheckboxes
                  courses={signInRecord.schedule || []}
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
  );
};

SignInForm.defaultProps = {
  reasons: []
};

export default SignInForm;
