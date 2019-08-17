import React from 'react';
import { Formik, Form, Field } from 'formik';
import ScaleLoader from 'react-spinners/ScaleLoader';
import * as Yup from 'yup';
import { Input, Button } from '../ui';
import { callApi, unwrapToJSON, ensureResponseCode } from '../utils';

const EmailSchema = Yup.object().shape({
  email: Yup.string()
    .email('Invalid email')
    .matches(/^[A-Z0-9._%+-]+@wvup.edu$/i, 'Must be a wvup email address')
    .trim()
    .required('Email is required')
});

const getStudentInfoWithEmail = email =>
  callApi(`signins/${email}/email`, 'GET', null);

// email or listen for card swipe, check banner on submit, go to next
// swipe id make auto submit
const EmailOrCardSwipeForm = ({ afterValidSubmit }) => {
  return (
    <Formik
      onSubmit={({ email }, { setSubmitting, setStatus }) => {
        getStudentInfoWithEmail(email)
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
      initialValues={{ email: '', id: 416467 }}
      validationSchema={EmailSchema}
    >
      {({ isSubmitting, isValid, status }) => (
        <Form>
          <h4>Please enter email or swipe card</h4>
          {status && <h4 style={{ color: 'red' }}>{status}</h4>}
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
            <Button type="Submit" disabled={!isValid} align="right">
              Submit
            </Button>
          )}
        </Form>
      )}
    </Formik>
  );
};

export default EmailOrCardSwipeForm;
