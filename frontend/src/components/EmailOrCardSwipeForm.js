import React, { useEffect } from 'react';
import type { Node } from 'react';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Input, Button, Stack } from '../ui';
import { callApi, unwrapToJSON, ensureResponseCode, isWVUPId } from '../utils';
import useCardReader from '../hooks/useCardReader';
import type { Teacher, Student } from '../types';

const EmailSchema = Yup.object().shape({
  email: Yup.string()
    .email('Invalid email')
    .matches(/^[A-Z0-9._%+-]+@wvup.edu$/i, 'Must be a wvup email address')
    .trim()
    .required('Email is required')
});

const getStudentInfoWithEmail = email =>
  callApi(`signins/${email}/email`, 'GET', null);

const getStudentInfoWithId = (id: number) =>
  callApi(`signins/${id}/id`, 'GET', null);

const getTeacherInfoWithEmail = email =>
  callApi(`signins/${email}/teacher/email`, 'GET', null);

const getTeacherInfoWithId = (id: number) =>
  callApi(`signins/${id}/teacher/id`, 'GET', null);

type Props = {
  afterValidSubmit: (Teacher & Student) => Promise<any>,
  teacher?: boolean,
  children?: Node
};

const EmailOrCardSwipeForm = ({
  afterValidSubmit,
  teacher,
  children
}: Props) => {
  const [data] = useCardReader();

  const getInfoWithEmail = teacher
    ? getTeacherInfoWithEmail
    : getStudentInfoWithEmail;

  const getInfoWithId = teacher ? getTeacherInfoWithId : getStudentInfoWithId;

  useEffect(() => {
    let isMounted = true;
    if (data && data.length > 2) {
      const wvupId = data.find(isWVUPId) || -1;
      getInfoWithId(wvupId)
        .then(ensureResponseCode(200))
        .then(unwrapToJSON)
        .then(personInfo => {
          if (isMounted) afterValidSubmit(personInfo);
        });
    }
    return () => {
      isMounted = false;
    };
  }, [data, afterValidSubmit, getInfoWithId]);

  return (
    <Formik
      onSubmit={({ email }, { setStatus }) => {
        return getInfoWithEmail(email)
          .then(ensureResponseCode(200))
          .then(unwrapToJSON)
          .then(afterValidSubmit)
          .catch(e => {
            if (e.message) {
              setStatus(e.message);
            }
          });
      }}
      initialValues={{ email: '' }}
      validationSchema={EmailSchema}
      isInitialValid={false}
    >
      {({ isSubmitting, isValid, status }) => {
        return (
          <Form>
            <Stack>
              {children}
              <h4>Please enter email or swipe card</h4>
              {status && <h4 style={{ color: 'red' }}>{status}</h4>}
              <Field
                id="email"
                type="text"
                name="email"
                component={Input}
                label="Email"
                disabled={isSubmitting}
              />
              <Button
                type="submit"
                disabled={!isValid || isSubmitting}
                fullWidth
                intent="primary"
              >
                {isSubmitting ? 'Submitting...' : 'Submit'}
              </Button>
            </Stack>
          </Form>
        );
      }}
    </Formik>
  );
};

EmailOrCardSwipeForm.defaultProps = {
  children: null,
  teacher: false
};

export default EmailOrCardSwipeForm;
