import React, { useState, useEffect } from 'react';
import styled from 'styled-components';
import { Formik, Form, Field } from 'formik';
import { pipe } from 'ramda';
import * as Yup from 'yup';
import {
  Card,
  Input,
  Header,
  Button,
  FormikDateTimePicker,
  Stack
} from '../../ui';
import useQuery from '../../hooks/useQuery';
import { callApi, unwrapToJSON, ensureResponseCode } from '../../utils';
import CoursesCheckboxes from '../../components/CoursesCheckboxes';
import ReasonCheckboxes from '../../components/ReasonCheckboxes';
import SignInSchema from '../../schemas/SignInFormSchema';

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

const getStudentInfoWithEmail = email =>
  callApi(`signins/${email}/email`, 'GET', null);

const getReasons = () => callApi(`reasons`, 'GET', null);

const queryReasons = pipe(getReasons, ensureResponseCode(200), unwrapToJSON);

const isWVUPEmail = email => email.match(/^[A-Z0-9._%+-]+@wvup.edu$/i);

const reduceToUniqueClasses = (acc, curr) => {
  const found = acc.find(ele => ele.crn === curr.crn);
  if (!found) acc.push(curr);
  return acc;
};

const loadClassList = email =>
  getStudentInfoWithEmail(email)
    .then(ensureResponseCode(200))
    .then(unwrapToJSON)
    .catch(e => alert(e.message));

const defaultData = {
  email: '',
  inTime: '',
  outTime: '',
  reasons: [],
  courses: [],
  tutoring: false
};

const crudTypes = {
  create: 'create',
  update: 'update'
};

// test email = mtmqbude26@wvup.edu
const SignIn = ({ data = defaultData, type = crudTypes.create }) => {
  const [reasons] = useQuery(queryReasons);
  const [email, setEmail] = useState(data.email);
  const [student, setStudent] = useState(data);
  const [classes, setClasses] = useState(student.courses);

  useEffect(() => {
    if (email === '') return;
    loadClassList(email).then(studentInfo => {
      // Class loaded could have a repeat class in it, reduce to only unique classes
      setStudent({ ...student, ...studentInfo });
      setClasses(
        [...classes, ...studentInfo.classSchedule].reduce(
          reduceToUniqueClasses,
          []
        )
      );
    });
  }, [email]);

  return (
    <FullScreenContainer>
      <Card style={{ paddingBottom: 0 }}>
        <Formik
          initialValues={{
            email: student.email,
            reasons: student.reasons.map(reason => reason.id),
            tutoring: student.tutoring,
            courses: student.courses.map(course => course.crn),
            inTime: student.inTime,
            outTime: student.outTime
          }}
          validationSchema={SignInUpdateSchema}
          onSubmit={async (values, { setSubmitting }) => {
            // massage data back into server format
            const signIn = {
              ...values,
              id: student.id,
              personId: student.studentID,
              semesterId: student.semesterId,
              courses: values.courses.map(courseCRN =>
                classes.find(ele => ele.crn === courseCRN)
              ),
              reasons: values.reasons.map(id =>
                reasons.find(ele => ele.id === id)
              )
            };

            switch (type) {
              case crudTypes.create:
                return postSignIn(signIn)
                  .then(ensureResponseCode(201))
                  .then(() => {
                    alert('Successfuly created');
                    window.history.back();
                  })
                  .catch(e => alert(e.message))
                  .finally(() => setSubmitting(false));
              case crudTypes.update:
                return putSignIn(signIn)
                  .then(ensureResponseCode(200))
                  .then(() => {
                    alert('Successfuly updated');
                    window.history.back();
                  })
                  .catch(e => alert(e.message))
                  .finally(() => setSubmitting(false));
              default:
                return () => 'Failed';
            }
          }}
        >
          {({
            values,
            isSubmitting,
            status,
            isValid,
            handleChange,
            errors
          }) => (
            <Form>
              <Stack>
                <Header>Student Sign In</Header>
                <div>KEY: Green = Active, Red = Deleted</div>
                {type === crudTypes.create && (
                  <p>Enter in Email to load classlist</p>
                )}
                {status && status.msg && <div>{status.msg}</div>}
                <Field
                  id="email"
                  type="email"
                  name="email"
                  component={Input}
                  label="Email"
                  onChange={e => {
                    handleChange(e);
                    if (isWVUPEmail(e.target.value)) setEmail(e.target.value);
                  }}
                  disabled={type !== crudTypes.create}
                />
                <DateInputGroup>
                  <Field
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
                  />
                </DateInputGroup>
                {reasons && (
                  <StyledReasonCheckboxes
                    reasons={reasons}
                    values={values}
                    errors={errors}
                  />
                )}
                {student && (
                  <CoursesCheckboxes courses={classes} errors={errors} />
                )}
                <Button
                  type="submit"
                  disabled={isSubmitting || !isValid}
                  intent="primary"
                  fullWidth
                >
                  Submit
                </Button>
              </Stack>
            </Form>
          )}
        </Formik>
      </Card>
    </FullScreenContainer>
  );
};

export default SignIn;
