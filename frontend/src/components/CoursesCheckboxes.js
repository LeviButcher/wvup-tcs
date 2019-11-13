import React from 'react';
import styled from 'styled-components';
import { Header, FieldGroup, Checkbox, Stack } from '../ui';

const SmallText = styled.span`
  color: #aaa;
  font-size: 0.8em;
  font-weight: normal;
`;

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
