import React, { useState } from 'react';
import SignInForm from '../../components/SignInForm';
import EmailOrCardSwipeForm from '../../components/EmailOrCardSwipeForm';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';
import { Card } from '../../ui';

const CreateSignIn = () => {
  const [person, setPerson]: [any, any] = useState();
  const [, { body: reasons }] = useApiWithHeaders('reasons/active');

  return (
    <Card>
      {!person ? (
        <div>
          <h1>Lookup a Student or Teacher</h1>
          <EmailOrCardSwipeForm
            afterValidSubmit={value => Promise.resolve(setPerson(value))}
          />
        </div>
      ) : (
        <div>
          <SignInForm
            signInRecord={{
              semesterId: '',
              email: person.email,
              schedule: person.schedule,
              inTime: '',
              outTime: '',
              selectedReasons: [],
              selectedClasses: [],
              personId: person.id,
              personType: person.personType,
              tutoring: false
            }}
            reasons={reasons}
          />
        </div>
      )}
    </Card>
  );
};

export default CreateSignIn;
