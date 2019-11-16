import React, { useEffect, useReducer } from 'react';
import { Formik, Form, Field } from 'formik';
import ScaleLoader from 'react-spinners/ScaleLoader';
import * as Yup from 'yup';
import { Input, Button, Stack } from '../ui';
import { callApi, unwrapToJSON, ensureResponseCode, isWVUPId } from '../utils';
import useCardReader from '../hooks/useCardReader';
import { loadingStates, loadingReducer } from '../hooks/loadingReducer';

const EmailSchema = Yup.object().shape({
  email: Yup.string()
    .email('Invalid email')
    .matches(/^[A-Z0-9._%+-]+@wvup.edu$/i, 'Must be a wvup email address')
    .trim()
    .required('Email is required')
});

const getStudentInfoWithEmail = email =>
  callApi(`signins/${email}/email`, 'GET', null);

const getStudentInfoWithId = id => callApi(`signins/${id}/id`, 'GET', null);

const getTeacherInfoWithEmail = email =>
  callApi(`signins/${email}/teacher/email`, 'GET', null);

const getTeacherInfoWithId = id =>
  callApi(`signins/${id}/teacher/id`, 'GET', null);

const EmailOrCardSwipeForm = ({
  afterValidSubmit,
  teacher
}: {
  afterValidSubmit: Function,
  teacher: boolean
}) => {
  const [data] = useCardReader();
  const [{ loading, errors }, dispatch] = useReducer(loadingReducer, {});

  const getInfoWithEmail = teacher
    ? getTeacherInfoWithEmail
    : getStudentInfoWithEmail;

  const getInfoWithId = teacher ? getTeacherInfoWithId : getStudentInfoWithId;
  useEffect(() => {
    let isMounted = true;
    if (data && data.length > 2) {
      const wvupId = data.find(isWVUPId);
      dispatch({ type: loadingStates.loading });
      getInfoWithId(wvupId)
        .then(ensureResponseCode(200))
        .then(unwrapToJSON)
        .then(personInfo => {
          afterValidSubmit(personInfo);
          if (isMounted) dispatch({ type: loadingStates.done });
        })
        .catch(
          e => isMounted && dispatch({ type: loadingStates.error, errors: e })
        );
    }
    return () => {
      isMounted = false;
    };
  }, [data, afterValidSubmit, getInfoWithId]);

  return (
    <Formik
      onSubmit={({ email }, { setSubmitting, setStatus }) => {
        getInfoWithEmail(email)
          .then(ensureResponseCode(200))
          .then(unwrapToJSON)
          .then(afterValidSubmit)
          .catch(e => {
            if (e.message) {
              setStatus(e.message);
            }
          })
          .finally(() => setSubmitting(false));
      }}
      initialValues={{ email: '' }}
      validationSchema={EmailSchema}
      enableReinitialize
    >
      {({ isSubmitting, isValid, status }) => (
        <Form>
          {(isSubmitting || loading) && (
            <Stack>
              <h5>Getting information...</h5>
              <ScaleLoader
                sizeUnit="px"
                size={150}
                loading={isSubmitting || loading}
                align="center"
              />
            </Stack>
          )}
          {errors && <h4 style={{ color: 'red' }}>{errors.message}</h4>}
          {!isSubmitting && !loading && (
            <Stack>
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
                type="Submit"
                disabled={!isValid && isSubmitting}
                fullWidth
                intent="primary"
              >
                Submit
              </Button>
            </Stack>
          )}
        </Form>
      )}
    </Formik>
  );
};

export default EmailOrCardSwipeForm;
