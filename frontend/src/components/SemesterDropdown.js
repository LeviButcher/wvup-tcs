import React from 'react';
import { Field } from 'formik';
import type { Semester } from '../types';

type Props = {
  semesters?: Array<Semester>
};

const SemesterDropdown = ({ semesters }: Props) => (
  <Field
    id="semesterCode"
    name="semesterCode"
    component="select"
    label="Semester"
    data-testid="semester-select"
  >
    <option style={{ display: 'none' }}>Select a Value</option>
    {semesters &&
      semesters
        .sort((a, b) => b.code - a.code)
        .map(({ code, name }) => (
          <option value={code} key={code}>
            {name}
          </option>
        ))}
  </Field>
);

SemesterDropdown.defaultProps = {
  semesters: []
};

export default SemesterDropdown;
