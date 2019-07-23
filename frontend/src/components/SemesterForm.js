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
  ensureResponseCode(200),
  unwrapToJSON
);

const SemesterForm = ({ onSubmit, name, ...props }) => {
  const [semesters] = useQuery(querySemesters);
  return (
    <Card {...props}>
      <Header>{name} Report</Header>
      <p>Choose Semester to create a report for</p>
      <Formik onSubmit={onSubmit} validationSchema={semesterSchema}>
        {({ isSubmitting, isValid }) => (
          <Form>
            <Field
              id="semester"
              name="semester"
              component="select"
              label="Semester"
              defaultValue="None"
            >
              <option style={{ display: 'none' }}>Select a Value</option>
              {semesters &&
                semesters
                  .sort((a, b) => b.id - a.id)
                  .map(semester => (
                    <option value={semester.id} key={semester.id}>
                      {semester.name}
                    </option>
                  ))}
            </Field>
            <Button
              type="submit"
              align="right"
              intent="primary"
              disabled={isSubmitting || !isValid}
            >
              Run Report
            </Button>
          </Form>
        )}
      </Formik>
    </Card>
  );
};

export default SemesterForm;
