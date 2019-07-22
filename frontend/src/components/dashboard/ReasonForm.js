import React from 'react';
import { Form, Field, Formik } from 'formik';
import { navigate } from '@reach/router';
import { Input, Button, Header, Card } from '../../ui';
import {
  callApi,
  ensureResponseCode,
  unwrapToJSON,
  errorToMessage
} from '../../utils';

const postReason = callApi(`reasons/`, 'POST');
const putReason = reason => callApi(`reasons/${reason.id}`, 'PUT', reason);

function createReason(values, { setSubmitting, setStatus }) {
  postReason(values)
    .then(ensureResponseCode(201))
    .then(unwrapToJSON)
    .then(() => {
      alert(`Created reason - ${values.name}`);
      navigate('/dashboard/admin/reason');
    })
    .catch(errorToMessage)
    .then(setStatus)
    .finally(() => {
      setSubmitting(false);
    });
}

function updateReason(values, { setSubmitting, setStatus }) {
  putReason(values)
    .then(ensureResponseCode(200))
    .then(unwrapToJSON)
    .then(() => {
      alert(`Updated reason - ${values.name}`);
      navigate('/dashboard/admin/reason');
    })
    .catch(errorToMessage)
    .then(setStatus)
    .finally(() => {
      setSubmitting(false);
    });
}

function getSubmitForAction(action) {
  switch (action) {
    case 'Create':
      return createReason;
    case 'Update':
      return updateReason;
    default:
      return () =>
        console.error(
          'Never hit a case in transalating an action to a submittion function'
        );
  }
}

const reasonDefault = {
  name: '',
  deleted: false
};

// do create and updates tours
const ReasonForm = ({ data = reasonDefault, action = 'Create' }) => {
  return (
    <Card style={{ margin: 'auto' }}>
      <Formik initialValues={data} onSubmit={getSubmitForAction(action)}>
        {({ values, status, isSubmitting }) => (
          <Form>
            <Header>{action} Reason</Header>
            {status && status.msg && (
              <div style={{ color: 'red' }}>{status.msg}</div>
            )}
            <Field
              id="name"
              type="text"
              name="name"
              component={Input}
              label="Name"
              required
            />
            <label htmlFor="deleted">Inactive</label>
            <Field
              id="deleted"
              type="checkbox"
              name="deleted"
              label="deleted"
              checked={values.deleted}
              disabled={action === 'Create'}
            />
            <Button align="right" disabled={isSubmitting} type="Submit">
              {action}
            </Button>
          </Form>
        )}
      </Formik>
    </Card>
  );
};

export default ReasonForm;
