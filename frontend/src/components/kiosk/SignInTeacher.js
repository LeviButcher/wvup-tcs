import React from 'react';
import { navigate } from '@reach/router';
import styled from 'styled-components';
import EmailForm from '../EmailForm';
import callApi from '../../utils/callApi';
import ensureResponseCode from '../../utils/ensureResponseCode';

const postSignOut = email =>
  callApi(
    `${process.env.REACT_APP_BACKEND}signins/${email}/signout`,
    'PUT',
    null
  );

// test email : teacher@wvup.edu
const SignInTeacher = () => {
  const handleSubmit = async (values, { setSubmitting, setStatus }) => {
    postSignOut(values.email)
      .then(ensureResponseCode(200))
      .then(() => {
        alert('You have signed in!');
        navigate('/');
      })
      .catch(e => {
        setStatus({ msg: e.message });
      })
      .finally(() => {
        setSubmitting(false);
      });
  };

  return (
    <FullScreenContainer>
      <EmailForm title="Teacher Sign In" onSubmit={handleSubmit} />
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
