import React from 'react';
import { Form, Field, Formik } from 'formik';
import * as Yup from 'yup';
import { navigate } from '@reach/router';
import { Input, Button, Header, Card, Stack } from '../ui';
import { callApi, ensureResponseCode } from '../utils';
import type { Reason } from '../types';

const ReasonSchema = Yup.object().shape({
  name: Yup.string()
    .trim()
    .required('Name is Required'),
  deleted: Yup.boolean().required()
});

const postReason = callApi(`reasons/`, 'POST');
const putReason = reason => callApi(`reasons/${reason.id}`, 'PUT', reason);

const createReason = values =>
  postReason(values)
    .then(ensureResponseCode(201))
    .then(() => {
      alert(`Created reason - ${values.name}`);
    });

const updateReason = values =>
  putReason(values)
    .then(ensureResponseCode(200))
    .then(() => {
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
        onSubmit={(submittedReason, { setStatus }) =>
          callCorrectApi(submittedReason)
            .then(() => navigate('/dashboard/admin/reason'))
            .catch(e => setStatus(e.message))
        }
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
