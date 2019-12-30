import React from 'react';
import { Formik, Form } from 'formik';
import * as Yup from 'yup';
import { Card, Header, Button, Stack } from '../ui';
import useApi from '../hooks/useApi';
import SemesterDropdown from './SemesterDropdown';

const semesterSchema = Yup.object().shape({
  semesterCode: Yup.string().required()
});

type Props = {
  onSubmit: (any, any) => Promise<any> & any,
  title: string,
  initialValues?: {
    semesterCode: string
  }
};

const SemesterForm = ({ onSubmit, title, initialValues, ...props }: Props) => {
  const [, semesters] = useApi('semesters');
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
              <SemesterDropdown semesters={semesters || []} />
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
