import React from 'react';
import styled from 'styled-components';
import { Formik, Form, Field } from 'formik';
import { Card, Input, Header, Button, FieldGroup } from '../../ui';

const Reasons = [
  { name: 'Computer Use', id: 1 },
  { name: 'Bone Use', id: 2 },
  { name: 'Lab Use', id: 3 },
  { name: 'Misc', id: 4 }
];

const Courses = [
  { courseName: 'CS121', CRN: '31345' },
  { courseName: 'EDUC101', CRN: '23456' },
  { courseName: 'GBUS304', CRN: '78924' },
  { courseName: 'SEC300', CRN: '65798' },
  { courseName: 'CS129', CRN: '32156' }
];

const SignIn = () => (
  <FullScreenContainer>
    <StyledCard>
      <Formik
        initialValues={{ email: '' }}
        validate={values => {
          const errors = {};
          if (!values.email) {
            errors.email = 'Required';
          } else if (
            !/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(values.email)
          ) {
            errors.email = 'Invalid email address';
          }
          return errors;
        }}
        onSubmit={(values, { setSubmitting }) => {
          setTimeout(() => {
            setSubmitting(false);
          }, 1000);
        }}
      >
        {({ isSubmitting }) => (
          <Form>
            <Header>Student Sign In</Header>
            <Field
              id="email"
              type="email"
              name="email"
              component={Input}
              label="Email"
            />
            <Header type="h4">Reason for Visiting</Header>
            <FieldGroup>
              <Field
                id="tutoring"
                type="checkbox"
                name="reason"
                component={Input}
                label="Tutoring"
                value="Tutoring"
              />
              {Reasons.map(reason => (
                <Field
                  key={reason.id}
                  id={reason.id}
                  type="checkbox"
                  name="reason"
                  component={Input}
                  label={reason.name}
                  value={reason.id}
                />
              ))}
            </FieldGroup>
            <Header type="h4">Classes visisiting for</Header>
            <FieldGroup>
              {Courses.map(course => (
                <Field
                  key={course.CRN}
                  id={course.CRN}
                  type="checkbox"
                  name="course"
                  component={Input}
                  label={course.courseName}
                  value={course.CRN}
                />
              ))}
            </FieldGroup>
            <Button
              type="submit"
              display="block"
              disabled={isSubmitting}
              style={{ marginLeft: 'auto' }}
            >
              Submit
            </Button>
          </Form>
        )}
      </Formik>
    </StyledCard>
  </FullScreenContainer>
);

const StyledCard = styled(Card)`
  width: 600px;
  padding: 2rem;
  box-shadow: 0 0 5px 1px;
`;

const FullScreenContainer = styled.div`
  padding: ${props => props.theme.padding};
  height: calc(100vh - 75px);
  display: flex;
  align-items: flex-start;
  justify-content: space-evenly;
`;

export default SignIn;
