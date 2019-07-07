import React from 'react';
import styled from 'styled-components';
import EmailForm from '../EmailForm';

const SignOut = () => (
  <FullScreenContainer>
    <EmailForm
      title="Sign Out"
      onSubmit={(values, { setSubmitting }) => {
        setTimeout(() => {
          setSubmitting(false);
        }, 1000);
      }}
    />
  </FullScreenContainer>
);

const FullScreenContainer = styled.div`
  padding: ${props => props.theme.padding};
  height: calc(100vh - ${props => props.theme.kioskHeaderSize});
  display: flex;
  align-items: center;
  justify-content: center;
`;

export default SignOut;
