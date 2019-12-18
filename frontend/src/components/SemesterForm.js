import React from 'react';
import { Formik, Form } from 'formik';
import { pipe } from 'ramda';
import * as Yup from 'yup';
import { Card, Header, Button, Stack } from '../ui';
import useQuery from '../hooks/useQuery';
import callApi from '../utils/callApi';
import ensureResponseCode from '../utils/ensureResponseCode';
import unwrapToJSON from '../utils/unwrapToJSON';
import SemesterDropdown from './SemesterDropdown';

const semesterSchema = Yup.object().shape({
  semesterCode: Yup.string().required()
});

const getSemesters = () => callApi(`reports/semesters`, 'GET', null);

const querySemesters = pipe(
  getSemesters,
  // $FlowFixMe
  ensureResponseCode(200),
  unwrapToJSON
);

type Props = {
  onSubmit: (any, any) => Promise<any> & any,
  title: string,
  initialValues?: {
    semesterCode: string
  }
};

const SemesterForm = ({ onSubmit, title, initialValues, ...props }: Props) => {
  const [semesters] = useQuery(querySemesters);
  return (
    <Card {...props}>
      <Header>{title}</Header>
      <p>Choose Semester to query for</p>
      <Formik
        onSubmit={onSubmit}
        validationSchema={semesterSchema}
        initialValues={initialValues}
        enableReinitialize
        isInitialValid={false}
      >
        {({ isSubmitting, isValid }) => (
          <Form>
            <Stack>
              <SemesterDropdown semesters={semesters} />
              <Button
                type="submit"
                fullWidth
                intent="primary"
                disabled={isSubmitting || !isValid}
              >
                Submit
              </Button>
            </Stack>
          </Form>
        )}
      </Formik>
    </Card>
  );
};

SemesterForm.defaultProps = {
  initialValues: {
    semesterCode: ''
  }
};

export default SemesterForm;
