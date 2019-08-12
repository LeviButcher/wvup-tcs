import React, { useState, useEffect } from 'react';
import styled from 'styled-components';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Card, Input, Header, Button } from '../../ui';
import { callApi, ensureResponseCode, unwrapToJSON } from '../../utils';

const SignInUpdateSchema = Yup.object()
  .shape({
    email: Yup.string()
      .email('Invalid email')
      .matches(/^[A-Z0-9._%+-]+@wvup.edu$/i, 'Must be a wvup email address')
      .trim()
      .required('Email is required'),
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

const putSignIn = signIn =>
  callApi(`signIns/${signIn.id}?teacher=true`, 'PUT', signIn);
const postSignIn = signIn =>
  callApi(`signIns/admin/?teacher=true`, 'POST', signIn);

const getTeacherInfo = email =>
  callApi(`signins/${email}/teacher/email`, 'GET', null);

const defaultData = {
  email: '',
  inTime: '',
  outTime: '',
  reasons: [],
  courses: [],
  tutoring: false
};

const isWVUPEmail = email => email.match(/^[A-Z0-9._%+-]+@wvup.edu$/i);

const crudTypes = {
  create: 'create',
  update: 'update'
};

// test email = mtmqbude26@wvup.edu
const SignInTeacher = ({ data = defaultData, type = crudTypes.create }) => {
  const [email, setEmail] = useState('');
  const [signInTeacherRecord, setSignInTeacher] = useState(data);

  useEffect(() => {
    if (email === '') return;
    getTeacherInfo(email)
      .then(ensureResponseCode(200))
      .then(unwrapToJSON)
      .then(setSignInTeacher);
  }, [email]);

  return (
    <FullScreenContainer>
      <Card>
        <Formik
          initialValues={{
            email: signInTeacherRecord.email,
            inTime: signInTeacherRecord.inTime,
            outTime: signInTeacherRecord.outTime,
            teacher: true
          }}
          validationSchema={SignInUpdateSchema}
          onSubmit={async (values, { setSubmitting }) => {
            // massage data back into server format
            console.log(signInTeacherRecord);
            const signIn = {
              ...values,
              id: signInTeacherRecord.id,
              personId:
                signInTeacherRecord.teacherID || signInTeacherRecord.personId,
              semesterId: signInTeacherRecord.semesterId
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
          {({ isSubmitting, status, isValid, handleChange }) => (
            <Form>
              <Header>Teacher Sign In</Header>
              {status && status.msg && <div>{status.msg}</div>}
              <Field
                id="email"
                type="email"
                name="email"
                component={Input}
                onChange={e => {
                  handleChange(e);
                  if (isWVUPEmail(e.target.value)) setEmail(e.target.value);
                }}
                label="Email"
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

export default SignInTeacher;
