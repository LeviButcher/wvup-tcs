import React, { useReducer } from 'react';
import { Link } from '@reach/router';
import styled from 'styled-components';
import ScaleLoader from 'react-spinners/ScaleLoader';
import { callApi, ensureResponseCode } from '../../utils';
import { Card } from '../../ui';
import EmailOrCardSwipeForm from '../../components/EmailOrCardSwipeForm';
import { loadingStates, loadingReducer } from '../../hooks/loadingReducer';

const postSignInTeacher = callApi(`signins?teacher=true`, 'POST');

// test email : teacher@wvup.edu
const SignInTeacher = ({ navigate }) => {
  const [{ loading, errors }, dispatch] = useReducer(loadingReducer, {});
  return (
    <FullScreenContainer>
      <Card>
        <Link to="/">Go to Home Screen</Link>
        <h1>Sign In Teacher</h1>
        {!loading && (
          <EmailOrCardSwipeForm
            teacher
            afterValidSubmit={teacher => {
              dispatch({ type: loadingStates.loading });
              const signIn = {
                ...teacher,
                personId: teacher.teacherID,
                email: teacher.teacherEmail
              };
              postSignInTeacher(signIn)
                .then(ensureResponseCode(201))
                .then(() => {
                  dispatch({ type: loadingStates.done });
                  navigate('/', { state: { info: 'You have signed in!' } });
                })
                .catch(e => dispatch({ type: loadingStates.error, errors: e }));
            }}
          />
        )}
        {loading && (
          <div>
            <h5>Submitting your signin</h5>
            <ScaleLoader
              sizeUnit="px"
              size={150}
              loading={loading}
              align="center"
            />
          </div>
        )}
        {errors && <div>{errors.message}</div>}
      </Card>
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
