import React from 'react';
import { Form, Field, Formik } from 'formik';
import * as Yup from 'yup';
import { navigate } from '@reach/router';
import { Input, Button, Header, Card, Stack } from '../ui';
import { apiFetch } from '../utils/fetchLight';
import type { Reason } from '../types';

const ReasonSchema = Yup.object().shape({
  name: Yup.string()
    .trim()
    .required('Name is Required'),
  deleted: Yup.boolean().required()
});

const createReason = values =>
  apiFetch(`reasons/`, 'POST', values).then(() => {
    alert(`Created reason - ${values.name}`);
  });

const updateReason = values =>
  apiFetch(`reasons/${values.id}`, 'PUT', values).then(() => {
    alert(`Updated reason - ${values.name}`);
  });

const reasonDefault = {
  name: '',
  deleted: false
};

type Props = {
  reason?: Reason
};

// do create and updates tours
const ReasonForm = ({ reason = reasonDefault }: Props) => {
  const action = reason === reasonDefault ? 'Create' : 'Update';

  const callCorrectApi = values => {
    if (action === 'Create') {
      return createReason(values);
    }
    return updateReason(values);
  };

  return (
    <Card style={{ margin: 'auto' }}>
      <Formik
        initialValues={reason}
        validationSchema={ReasonSchema}
        onSubmit={(submittedReason, { setStatus }) => {
          // $FlowFixMe
          return callCorrectApi(submittedReason)
            .then(() => navigate('/dashboard/admin/reason'))
            .catch(e => e.unwrapFetchErrorMessage())
            .then(m => setStatus(m));
        }}
        isInitialValid={false}
      >
        {({ status, isSubmitting, isValid }) => (
          <Form>
            <Stack>
              <Header>{action} Reason</Header>
              {status && <div style={{ color: 'red' }}>{status}</div>}
              <Field
                id="name"
                type="text"
                name="name"
                component={Input}
                label="Name"
                required
              />
              <Field
                id="deleted"
                type="checkbox"
                name="deleted"
                label="deleted"
                component={Input}
                hidden={action === 'Create'}
              />
              <Button
                align="right"
                disabled={isSubmitting || !isValid}
                type="Submit"
              >
                {isSubmitting ? 'Submitting...' : 'Submit'}
              </Button>
            </Stack>
          </Form>
        )}
      </Formik>
    </Card>
  );
};

ReasonForm.defaultProps = {
  reason: reasonDefault
};

export default ReasonForm;
