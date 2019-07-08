import React from 'react';
import styled from 'styled-components';
import { navigate } from '@reach/router';
import EmailForm from '../EmailForm';
import callApi from '../../utils/callApi';

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
      onSubmit={(values, { setSubmitting }) => {
        const { email } = values;
        putSignOut(email)
          .then(async res => {
            if (res.status === 200) {
              alert('You have signed out!');
              navigate('/');
            } else {
              const data = await res.json();
              throw Error(data);
            }
          })
          .catch(e => alert(e.message))
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
