import React from 'react';
import { Formik, Form, Field } from 'formik';
import { pipe } from 'ramda';
import * as Yup from 'yup';
import { Card, Header, Button } from '../ui';
import useQuery from '../hooks/useQuery';
import callApi from '../utils/callApi';
import ensureResponseCode from '../utils/ensureResponseCode';
import unwrapToJSON from '../utils/unwrapToJSON';

const semesterSchema = Yup.object().shape({
  semester: Yup.string().required()
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
    semester: string
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
            <Field
              id="semester"
              name="semester"
              component="select"
              label="Semester"
              data-testid="semester-select"
            >
              <option style={{ display: 'none' }}>Select a Value</option>
              {semesters &&
                semesters
                  .sort((a, b) => b.id - a.id)
                  .map(({ id, name: semesterName }) => (
                    <option value={id} key={id}>
                      {semesterName}
                    </option>
                  ))}
            </Field>
            <Button
              type="submit"
              fullWidth
              intent="primary"
              disabled={isSubmitting || !isValid}
            >
              Submit
            </Button>
          </Form>
        )}
      </Formik>
    </Card>
  );
};

SemesterForm.defaultProps = {
  initialValues: {
    semester: ''
  }
};

export default SemesterForm;
