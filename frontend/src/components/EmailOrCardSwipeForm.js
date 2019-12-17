import React, { useEffect } from 'react';
import type { Node } from 'react';
import { Formik, Form, Field, useFormikContext } from 'formik';
import * as Yup from 'yup';
import { Input, Button, Stack } from '../ui';
import { callApi, unwrapToJSON, ensureResponseCode, isWVUPId } from '../utils';
import useCardReader from '../hooks/useCardReader';
import type { Teacher, Student } from '../types';

const EmailSchema = Yup.object().shape({
  id: Yup.string(),
  email: Yup.string().when(
    'id',
    // $FlowFixMe
    {
      is: val => {
        return val && val !== '';
      },
      then: Yup.string(),
      otherwise: Yup.string()
        .email('Invalid email')
        .matches(/^[A-Z0-9._%+-]+@wvup.edu$/i, 'Must be a wvup email address')
        .trim()
        .required('Email is required')
    }
  )
});

// Can be called with either email or wvupId
const getPersonInfo = (identifier: string | number) =>
  callApi(`person/${identifier}`, 'GET', null);

type Props = {
  afterValidSubmit: (Teacher & Student) => Promise<any>,
  children?: Node
};

const FormikCardReader = () => {
  const [data] = useCardReader();
  const { setValues, submitForm, validateForm } = useFormikContext();

  useEffect(() => {
    let isMounted = true;
    if (data && data.length > 2) {
      const wvupId = data.find(isWVUPId) || -1;
      if (wvupId) {
        setValues({ id: wvupId });
        if (isMounted) validateForm().then(() => submitForm());
      }
    }
    return () => {
      isMounted = false;
    };
  }, [data, setValues, submitForm, validateForm]);

  return null;
};

const EmailOrCardSwipeForm = ({ afterValidSubmit, children }: Props) => {
  return (
    <Formik
      onSubmit={({ email, id }, { setStatus }) => {
        const identifier = id && id !== '' ? id : email;

        return getPersonInfo(identifier)
          .then(ensureResponseCode(200))
          .then(unwrapToJSON)
          .then(afterValidSubmit)
          .catch(e => {
            if (e.message) {
              setStatus(e.message);
            }
          });
      }}
      initialValues={{ email: '', id: '' }}
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
              <FormikCardReader />
            </Stack>
          </Form>
        );
      }}
    </Formik>
  );
};

EmailOrCardSwipeForm.defaultProps = {
  children: null
};

export default EmailOrCardSwipeForm;
