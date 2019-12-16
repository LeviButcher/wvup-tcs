import React from 'react';
import { Link, navigate } from '@reach/router';
import { callApi, ensureResponseCode } from '../../utils';
import { Card, KioskFullScreenContainer } from '../../ui';
import EmailOrCardSwipeForm from '../../components/EmailOrCardSwipeForm';
import type { Teacher } from '../../types';

const postSignInTeacher = callApi(`session/signin`, 'POST');

// test email : teacher@wvup.edu
const SignInTeacher = () => {
  return (
    <KioskFullScreenContainer>
      <Card>
        <EmailOrCardSwipeForm
          afterValidSubmit={(teacher: Teacher) => {
            const signIn = {
              personId: teacher.id
            };
            return postSignInTeacher(signIn)
              .then(ensureResponseCode(201))
              .then(() => {
                navigate('/', { state: { info: 'You have signed in!' } });
              });
          }}
        >
          <Link to="/">Go to Home Screen</Link>
          <h1>Sign In Teacher</h1>
        </EmailOrCardSwipeForm>
      </Card>
    </KioskFullScreenContainer>
  );
};

export default SignInTeacher;
