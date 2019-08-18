import React, { useEffect, useState } from 'react';
import styled from 'styled-components';
import { navigate } from '@reach/router';
import EmailForm from '../../components/EmailForm';
import callApi from '../../utils/callApi';
import ensureResponseCode from '../../utils/ensureResponseCode';
import useCardReader from '../../hooks/useCardReader';

const putSignOutEmail = email =>
  callApi(`signins/${email}/signout`, 'PUT', null);

const putSignOutId = id => callApi(`signins/${id}/signout`, 'PUT', null);

// test email: mtmqbude26@wvup.edu
const SignOutPage = () => {
  const [data] = useCardReader();
  const [errors, setErrors] = useState();

  useEffect(() => {
    if (data && data.length > 2) {
      const [, wvupId] = data;
      putSignOutId(wvupId)
        .then(ensureResponseCode(200))
        .then(() => {
          alert('You have signed out!');
          navigate('/');
        })
        .catch(setErrors);
    }
  }, [data]);

  return (
    <FullScreenContainer>
      <EmailForm
        title="Sign Out"
        errors={errors}
        onSubmit={({ email }, { setSubmitting, setStatus }) => {
          putSignOutEmail(email)
            .then(ensureResponseCode(200))
            .then(() => {
              alert('You have signed out!');
              navigate('/');
            })
            .catch(e => {
              console.log(e);
              setStatus({ msg: e.message });
            })
            .finally(() => setSubmitting(false));
        }}
      />
    </FullScreenContainer>
  );
};

const FullScreenContainer = styled.div`
  padding: ${props => props.theme.padding};
  height: calc(100vh - ${props => props.theme.kioskHeaderSize});
  display: flex;
  align-items: center;
  justify-content: center;
`;

export default SignOutPage;
