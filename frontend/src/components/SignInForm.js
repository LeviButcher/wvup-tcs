import React from 'react';
import styled from 'styled-components';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import {
  Card,
  Input,
  Header,
  Button,
  FormikDateTimePicker,
  Stack
} from '../ui';
import { callApi, ensureResponseCode } from '../utils';
import CoursesCheckboxes from './CoursesCheckboxes';
import ReasonCheckboxes from './ReasonCheckboxes';
import SignInSchema from '../schemas/SignInFormSchema';
import type { Course, Reason } from '../types';

function isBeforeInTime(outTime) {
  const inTime = this.resolve(Yup.ref('inTime'));
  return inTime < outTime;
}

const SignInUpdateSchema = SignInSchema.shape({
  inTime: Yup.date()
    .typeError('Invalid Date format')
    .required(),
  outTime: Yup.date()
    .typeError('Invalid Date format')
    .test('date-test', 'Must be after In Time', isBeforeInTime)
    .required()
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

const DateInputGroup = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-gap: 1rem;
  & div:last-child {
    text-align: right;
  }
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

const putSignIn = signIn => callApi(`signIns/${signIn.id}`, 'PUT', signIn);
const postSignIn = signIn => callApi(`signIns/admin/`, 'POST', signIn);

const reduceToUniqueClasses = (acc, curr) => {
  const found = acc.find(ele => ele.crn === curr.crn);
  if (!found) acc.push(curr);
  return acc;
};

const defaultData = {
  email: '',
  inTime: '',
  outTime: '',
  reasons: [],
  courses: [],
  tutoring: false
};

type Props = {
  signInRecord: {
    email: string,
    classSchedule: Array<Course>,
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
  reasons: Array<Reason>
};

// test email = mtmqbude26@wvup.edu
const SignInForm = ({ signInRecord = defaultData, reasons }: Props) => {
  return (
    <FullScreenContainer>
      <Card>
        <Formik
          initialValues={{
            email: signInRecord.email,
            reasons: signInRecord.selectedReasons,
            tutoring: signInRecord.tutoring,
            courses: signInRecord.selectedClasses,
            inTime: signInRecord.inTime,
            outTime: signInRecord.outTime
          }}
          isInitialValid={false}
          validationSchema={SignInUpdateSchema}
          onSubmit={values => {
            // massage data back into server format
            const signIn = {
              ...values,
              inTime: new Date(values.inTime),
              outTime: new Date(values.outTime),
              id: signInRecord.id,
              personId: signInRecord.personId,
              semesterId: signInRecord.semesterId,
              courses: values.courses.map(courseCRN =>
                signInRecord.selectedClasses.find(crn => crn === courseCRN)
              ),
              reasons: values.reasons.map(id =>
                reasons.find(ele => ele.id === id)
              )
            };

            if (signInRecord.personType === 'Student') {
              if (signInRecord.selectedClasses.length === 0) {
                return postSignIn(signIn)
                  .then(ensureResponseCode(201))
                  .then(() => {
                    alert('Successfuly created');
                    window.history.back();
                  })
                  .catch(e => alert(e.message));
              }
            }

            // switch (type) {
            //   case crudTypes.create:
            //     return postSignIn(signIn)
            //       .then(ensureResponseCode(201))
            //       .then(() => {
            //         alert('Successfuly created');
            //         window.history.back();
            //       })
            //       .catch(e => alert(e.message))
            //       .finally(() => setSubmitting(false));
            //   case crudTypes.update:
            //     return putSignIn(signIn)
            //       .then(ensureResponseCode(200))
            //       .then(() => {
            //         alert('Successfuly updated');
            //         window.history.back();
            //       })
            //       .catch(e => alert(e.message))
            //       .finally(() => setSubmitting(false));
            //   default:
            //     return () => 'Failed';
          }}
        >
          {({ values, isSubmitting, status, isValid, errors, touched }) => (
            <Form>
              <Stack>
                <Header>Student Sign In</Header>
                <div>KEY: Green = Active, Red = Deleted</div>
                {status && status.msg && <div>{status.msg}</div>}
                <Field
                  id="email"
                  type="email"
                  name="email"
                  component={Input}
                  label="Email"
                />
                <DateInputGroup>
                  {/* <Field
                    id="inTime"
                    name="inTime"
                    component={FormikDateTimePicker}
                    label="In Time"
                  />
                  <Field
                    id="outTime"
                    name="outTime"
                    component={FormikDateTimePicker}
                    label="Out Time"
                  /> */}
                </DateInputGroup>
                <StyledReasonCheckboxes
                  reasons={reasons}
                  values={values}
                  errors={errors}
                  touched={touched}
                />
                <CoursesCheckboxes
                  courses={signInRecord.classSchedule}
                  errors={errors}
                  touched={touched}
                />
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

export default SignInForm;
