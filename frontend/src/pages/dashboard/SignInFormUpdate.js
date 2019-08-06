import React, { useState, useEffect } from 'react';
import styled from 'styled-components';
import { Formik, Form, Field } from 'formik';
import { pipe } from 'ramda';
import * as Yup from 'yup';
import { Card, Input, Header, Button } from '../../ui';
import useQuery from '../../hooks/useQuery';
import { callApi, unwrapToJSON, ensureResponseCode } from '../../utils';
import CoursesCheckboxes from '../../components/CoursesCheckboxes';
import ReasonCheckboxes from '../../components/ReasonCheckboxes';
import SignInSchema from '../../schemas/SignInFormScema';

const SignInUpdateSchema = SignInSchema.shape({
  inTime: Yup.date()
    .typeError('Invalid Date format')
    .required(),
  outTime: Yup.date()
    .typeError('Invalid Date format')
    .test('date-test', 'Must be after In Time', function(outTime) {
      return this.resolve(Yup.ref('inTime')) < outTime;
    })
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

const StyledReasonCheckboxes = styled(ReasonCheckboxes)`
  & label[data-deleted='true'] {
    color: red;
  }
  & label[data-deleted='false'] {
    color: green;
  }
`;

const putSignIn = signIn => callApi(`signIns/${signIn.id}`, 'PUT', signIn);
const postSignIn = signIn => callApi(`signIns/admin`, 'POST', signIn);

const getStudentInfoWithEmail = email =>
  callApi(`signins/${email}/email`, 'GET', null);

const getReasons = () => callApi(`reasons`, 'GET', null);

const queryReasons = pipe(
  getReasons,
  ensureResponseCode(200),
  unwrapToJSON
);

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
    .then(res => res.classSchedule)
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
  const [student, setStudent] = useState(data);
  const [classes, setClasses] = useState(student.courses);

  useEffect(() => {
    if (student.email === '') return;
    loadClassList(student.email).then(currentCourses => {
      // Class loaded could have a repeat class in it, reduce to only unique classes
      setClasses(
        [...classes, ...currentCourses].reduce(reduceToUniqueClasses, [])
      );
    });
  }, [student]);

  console.log(student);
  return (
    <FullScreenContainer>
      <Card>
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
              personId: student.personId,
              semesterId: student.semesterId,
              courses: values.courses.map(courseCRN => ({
                courseId: courseCRN
              })),
              reasons: values.reasons.map(id => ({
                reasonId: id
              }))
            };

            console.log(signIn);
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
              <Header>Student Sign In</Header>
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
                  console.log(e.target.value);
                  if (isWVUPEmail(e.target.value))
                    setStudent({ ...student, email: e.target.value });
                }}
                disabled={type !== crudTypes.create}
              />
              <Field
                id="inTime"
                type="datetime-local"
                name="inTime"
                component={Input}
                label="In Time"
              />
              <Field
                id="outTime"
                type="datetime-local"
                name="outTime"
                component={Input}
                label="Out Time"
              />
              {reasons && (
                <StyledReasonCheckboxes
                  reasons={reasons}
                  values={values}
                  errors={errors}
                />
              )}
              {student && (
                <>
                  <CoursesCheckboxes courses={classes} errors={errors} />
                </>
              )}
              <br />
              <Button
                type="submit"
                align="right"
                disabled={isSubmitting || !isValid}
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

export default SignIn;
