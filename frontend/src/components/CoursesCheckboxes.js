import React from 'react';
import { Header, FieldGroup, Checkbox, Stack, SmallText } from '../ui';

const CoursesCheckboxes = ({ courses, errors }) => {
  return (
    <Stack>
      <Header type="h4">
        Classes Visiting for <SmallText>Select at least one course</SmallText>
        <div style={{ color: 'red' }}>{errors && errors.courses}</div>
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
