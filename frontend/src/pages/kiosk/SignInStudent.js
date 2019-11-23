import React, { useState } from 'react';
import { Link } from '@reach/router';
import { Card, KioskFullScreenContainer, Stack } from '../../ui';
import EmailOrCardSwipeForm from '../../components/EmailOrCardSwipeForm';
import StudentSignInForm from '../../components/StudentSignInForm';

// test email = mtmqbude26@wvup.edu
const SignInStudent = () => {
  const [student, setStudent] = useState();

  return (
    <KioskFullScreenContainer>
      <Card>
        <Stack>
          <Link to="/">Go to Home Screen</Link>
          <h1>Student SignIn</h1>
          {!student ? (
            <EmailOrCardSwipeForm
              default
              afterValidSubmit={studentInfo => {
                return Promise.resolve(setStudent(studentInfo));
              }}
            />
          ) : (
            <StudentSignInForm student={student} />
          )}
        </Stack>
      </Card>
    </KioskFullScreenContainer>
  );
};

export default SignInStudent;
