import React from 'react';
import styled from 'styled-components';
import { navigate } from '@reach/router';
import EmailForm from '../EmailForm';
import callApi from '../../utils/callApi';
import ensureResponseCode from '../../utils/ensureResponseCode';

const putSignOut = email =>
  callApi(
    `${process.env.REACT_APP_BACKEND}signins/${email}/signout`,
    'PUT',
    null
  );

// test email: mtmqbude26@wvup.edu
const SignOut = () => (
  <FullScreenContainer>
    <EmailForm
      title="Sign Out"
      onSubmit={({ email }, { setSubmitting, setStatus }) => {
        putSignOut(email)
          .then(ensureResponseCode(200))
          .then(() => {
            alert('You have signed out!');
            navigate('/');
          })
          .catch(e => setStatus({ msg: e.message }))
          .finally(() => setSubmitting(false));
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
