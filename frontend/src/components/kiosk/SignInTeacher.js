import React from 'react';
import styled from 'styled-components';
import EmailForm from '../EmailForm';

const SignInTeacher = () => {
  return (
    <FullScreenContainer>
      <EmailForm
        title="Teacher Sign In"
        onSubmit={(values, { setSubmitting }) => {
          setTimeout(() => {
            alert('Sign in for teacher is not connected to backend yet');
            setSubmitting(false);
          }, 1000);
        }}
      />
    </FullScreenContainer>
  );
};

const FullScreenContainer = styled.div`
  padding: ${props => props.theme.padding};
  height: calc(100vh - 75px);
  display: flex;
  align-items: center;
  justify-content: space-evenly;
`;

export default SignInTeacher;
