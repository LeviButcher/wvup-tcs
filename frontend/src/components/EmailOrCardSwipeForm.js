import React, { useEffect, useReducer } from 'react';
import { Formik, Form, Field } from 'formik';
import ScaleLoader from 'react-spinners/ScaleLoader';
import * as Yup from 'yup';
import { Input, Button } from '../ui';
import { callApi, unwrapToJSON, ensureResponseCode } from '../utils';
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

// email or listen for card swipe, check banner on submit, go to next
// swipe id make auto submit
const EmailOrCardSwipeForm = ({ afterValidSubmit, teacher }) => {
  const [data] = useCardReader();
  const [{ loading, errors }, dispatch] = useReducer(loadingReducer, {});

  const getInfoWithEmail = teacher
    ? getTeacherInfoWithEmail
    : getStudentInfoWithEmail;

  const getInfoWithId = teacher ? getTeacherInfoWithId : getStudentInfoWithId;

  useEffect(() => {
    let isMounted = true;
    if (data && data.length > 2) {
      const [, wvupId] = data;
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
        console.log(email);
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
        <>
          {loading && (
            <div>
              <h5>Getting information with Id...</h5>
              <ScaleLoader
                sizeUnit="px"
                size={150}
                loading={loading}
                align="center"
              />
            </div>
          )}
          <Form>
            <h4>Please enter email or swipe card</h4>
            {status && <h4 style={{ color: 'red' }}>{status}</h4>}
            {errors && <h4 style={{ color: 'red' }}>{errors.message}</h4>}
            <Field
              id="email"
              type="text"
              name="email"
              component={Input}
              label="Email"
              disabled={isSubmitting}
              autoFocus
            />
            {isSubmitting && <h5>Getting information...</h5>}
            <ScaleLoader
              sizeUnit="px"
              size={150}
              loading={isSubmitting}
              align="center"
            />
            {!isSubmitting && (
              <Button
                type="Submit"
                disabled={!isValid && isSubmitting}
                align="right"
              >
                Submit
              </Button>
            )}
          </Form>
        </>
      )}
    </Formik>
  );
};

export default EmailOrCardSwipeForm;
