import React from 'react';
import { Link, navigate } from '@reach/router';
import styled from 'styled-components';
import { callApi, ensureResponseCode } from '../../utils';
import { Card } from '../../ui';
import EmailOrCardSwipeForm from '../../components/EmailOrCardSwipeForm';

const postSignInTeacher = callApi(`signins?teacher=true`, 'POST');

// test email : teacher@wvup.edu
const SignInTeacher = () => {
  return (
    <FullScreenContainer>
      <Card>
        <EmailOrCardSwipeForm
          teacher
          afterValidSubmit={teacher => {
            const signIn = {
              personId: teacher.teacherID,
              email: teacher.teacherEmail
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
    </FullScreenContainer>
  );
};

// $FlowFixMe
const FullScreenContainer = styled.div`
  padding: ${props => props.theme.padding};
  height: calc(100vh - 75px);
  display: flex;
  align-items: center;
  justify-content: space-evenly;
`;

export default SignInTeacher;
