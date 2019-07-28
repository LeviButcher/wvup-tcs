import React from 'react';
import { ErrorMessage } from 'formik';
import styled from 'styled-components';
import { Header, FieldGroup, Checkbox } from '../ui';

const SmallText = styled.span`
  color: #aaa;
  font-size: 0.8em;
`;

const CoursesCheckboxes = ({ courses }) => {
  return (
    <>
      <Header type="h4">
        Classes Visiting for <SmallText>Select at least one course</SmallText>
        <ErrorMessage name="courses">
          {message => <div style={{ color: 'red' }}>{message}</div>}
        </ErrorMessage>
      </Header>
      <FieldGroup>
        {courses.map(course => (
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
    </>
  );
};

export default CoursesCheckboxes;
