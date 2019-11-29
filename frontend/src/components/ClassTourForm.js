import React from 'react';
import { Form, Field, Formik } from 'formik';
import { navigate } from '@reach/router';
import * as Yup from 'yup';
import { Input, Button, Header, Card, Stack } from '../ui';
import { callApi, ensureResponseCode } from '../utils';
import type { ClassTour } from '../types';

const ClassTourSchema = Yup.object().shape({
  name: Yup.string()
    .trim()
    .required(),
  dayVisited: Yup.string().required(),
  numberOfStudents: Yup.number()
    .positive()
    .required()
});

const postClassTour = callApi(`classtours/`, 'POST');

const createClassTour = values =>
  postClassTour(values).then(ensureResponseCode(201));

const updateClassTour = values =>
  callApi(`classtours/${values.id}`, 'PUT', values).then(
    ensureResponseCode(200)
  );

const classTourDefault = {
  name: '',
  dayVisited: '',
  numberOfStudents: 0
};

type Props = {
  classTour?: ClassTour
};

const ClassTourForm = ({ classTour }: Props) => {
  const action = classTour === classTourDefault ? 'Create' : 'Update';

  const callCorrectApi = values => {
    if (classTour === classTourDefault) {
      return createClassTour(values);
    }
    return updateClassTour(values);
  };

  return (
    <Card>
      <Formik
        initialValues={classTour}
        onSubmit={(submittedTour, { setStatus }) => {
          return callCorrectApi(submittedTour)
            .then(() => {
              alert(`Updated tour for ${submittedTour.name}`);
              navigate('/dashboard/tours');
            })
            .catch(e => {
              setStatus(e.message);
            });
        }}
        validationSchema={ClassTourSchema}
        isInitialValid={false}
      >
        {({ values, status, isSubmitting, isValid }) => (
          <Form>
            <Stack>
              <Header>{action} Tour</Header>
              {status && status && <div style={{ color: 'red' }}>{status}</div>}
              <Field
                id="name"
                type="text"
                name="name"
                component={Input}
                label="Name"
              />
              <Field
                id="dayVisited"
                type="date"
                name="dayVisited"
                value={
                  values.dayVisited.length > 0
                    ? new Date(values.dayVisited).toISOString().substr(0, 10)
                    : ''
                }
                component={Input}
                label="Date Toured"
              />
              <Field
                id="numberOfStudents"
                type="number"
                name="numberOfStudents"
                component={Input}
                label="Number of Tourist"
              />
              <Button
                type="submit"
                fullWidth
                disabled={isSubmitting || !isValid}
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

ClassTourForm.defaultProps = {
  classTour: classTourDefault
};

export default ClassTourForm;
