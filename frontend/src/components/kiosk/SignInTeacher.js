import React from 'react';
import { navigate } from '@reach/router';
import styled from 'styled-components';
import EmailForm from '../EmailForm';
import callApi from '../../utils/callApi';
import ensureResponseCode from '../../utils/ensureResponseCode';
import unWrapToJSON from '../../utils/unwrapToJSON';

const postSignInTeacher = callApi(
  `${process.env.REACT_APP_BACKEND}signins?teacher=true`,
  'POST'
);

const getTeacherInfoWithEmail = email =>
  callApi(
    `${process.env.REACT_APP_BACKEND}signins/${email}/teacher/email`,
    'GET',
    null
  );

// get teacher information
// transform that into signIn
// send to post

// test email : teacher@wvup.edu
const SignInTeacher = () => {
  const loadTeacherInfo = email =>
    getTeacherInfoWithEmail(email)
      .then(ensureResponseCode(200))
      .then(unWrapToJSON);

  const handleSubmit = async ({ email }, { setSubmitting, setStatus }) => {
    loadTeacherInfo(email)
      .catch(() => {
        throw new Error(`Can't find your information`);
      })
      .then(teacher => ({
        ...teacher,
        id: teacher.teacherId,
        email: teacher.teacherEmail
      }))
      .then(signIn => postSignInTeacher(signIn))
      .then(ensureResponseCode(201))
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
      <EmailForm title="Teacher Sign In" onSubmit={handleSubmit} disabled />
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
