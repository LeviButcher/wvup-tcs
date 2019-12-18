import React from 'react';
import { navigate, Link } from '@reach/router';
import { callApi } from '../../utils';
import ensureResponseCode from '../../utils/ensureResponseCode';
import { KioskFullScreenContainer, Card, Stack } from '../../ui';
import EmailOrCardSwipeForm from '../../components/EmailOrCardSwipeForm';

const putSignOut = (personId: string) =>
  callApi(`sessions/out`, 'PUT', { personId });

const SignOutPage = () => {
  return (
    <KioskFullScreenContainer>
      <Card>
        <Stack>
          <Link to="/">Go to Home Screen</Link>
          <h1>Sign Out</h1>
          <EmailOrCardSwipeForm
            afterValidSubmit={({ id }) => {
              return putSignOut(id)
                .then(ensureResponseCode(200))
                .then(() => {
                  navigate('/', { state: { info: 'You have signed out!' } });
                });
            }}
          />
        </Stack>
      </Card>
    </KioskFullScreenContainer>
  );
};

export default SignOutPage;
