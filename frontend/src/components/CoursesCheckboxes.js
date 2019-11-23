import React from 'react';
import { Header, FieldGroup, Checkbox, Stack, SmallText } from '../ui';
import type { Course } from '../types';

type Props = {
  courses: Array<Course>,
  errors: { courses: string },
  touched: { courses: string }
};

const CoursesCheckboxes = ({ courses, errors, touched }: Props) => {
  return (
    <Stack>
      <Header type="h4">
        Classes Visiting for <SmallText>Select at least one course</SmallText>
        {errors.courses && touched.courses && (
          <div style={{ color: 'red' }}>{errors.courses}</div>
        )}
      </Header>
      <FieldGroup>
        {courses &&
          courses.map(course => (
            <Checkbox
              key={course.crn}
              id={course.crn}
              type="checkbox"
              name="courses"
              label={course.shortName}
              value={course.crn}
            />
          ))}
      </FieldGroup>
    </Stack>
  );
};

export default CoursesCheckboxes;
