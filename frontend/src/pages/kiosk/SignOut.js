import React, { useEffect, useState } from 'react';
import { navigate } from '@reach/router';
import EmailForm from '../../components/EmailForm';
import { callApi, isWVUPId } from '../../utils';
import ensureResponseCode from '../../utils/ensureResponseCode';
import useCardReader from '../../hooks/useCardReader';
import { KioskFullScreenContainer } from '../../ui';

const putSignOutEmail = email =>
  callApi(`signins/${email}/signout`, 'PUT', null);

const putSignOutId = (id: number) =>
  callApi(`signins/${id}/signout`, 'PUT', null);

// test email: mtmqbude26@wvup.edu
const SignOutPage = () => {
  const [data] = useCardReader();
  const [errors, setErrors] = useState();

  useEffect(() => {
    if (data && data.length > 2) {
      const wvupId = data.find(isWVUPId) || -1;
      putSignOutId(wvupId)
        .then(ensureResponseCode(200))
        .then(() => {
          navigate('/', { state: { info: 'You have signed out!' } });
        })
        .catch(setErrors);
    }
  }, [data]);

  return (
    <KioskFullScreenContainer>
      <EmailForm
        title="Sign Out"
        errors={errors}
        onSubmit={({ email }, { setStatus }) => {
          return putSignOutEmail(email)
            .then(ensureResponseCode(200))
            .then(() => {
              navigate('/', { state: { info: 'You have signed out!' } });
            })
            .catch(e => {
              setStatus({ msg: e.message });
            });
        }}
      />
    </KioskFullScreenContainer>
  );
};

export default SignOutPage;
