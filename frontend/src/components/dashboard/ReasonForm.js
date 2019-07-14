import React from 'react';
import { Form, Field, Formik } from 'formik';
import { navigate } from '@reach/router';
import { Input, Button, Header, Card } from '../../ui';
import callApi from '../../utils/callApi';

const postReason = callApi(`${process.env.REACT_APP_BACKEND}reasons/`, 'POST');

const putReason = reason =>
  callApi(
    `${process.env.REACT_APP_BACKEND}reasons/${reason.id}`,
    'PUT',
    reason
  );

function createReason(values, { setSubmitting, setStatus }) {
  const message = {};
  postReason(values)
    .then(async res => {
      if (res.status !== 201) {
        throw await res.json();
      }
      alert(`Created reason - ${values.name}`);
      navigate('/dashboard/admin/reason');
    })
    .catch(e => {
      message.msg = e.message;
    })
    .finally(() => {
      setSubmitting(false);
      setStatus(message);
    });
}

function updateReason(values, { setSubmitting, setStatus }) {
  const message = {};
  putReason(values)
    .then(async res => {
      if (res.status !== 200) {
        throw await res.json();
      }
      alert(`Updated reason - ${values.name}`);
      navigate('/dashboard/admin/reason');
    })
    .catch(e => {
      message.msg = e.message;
    })
    .finally(() => {
      setSubmitting(false);
      setStatus(message);
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
