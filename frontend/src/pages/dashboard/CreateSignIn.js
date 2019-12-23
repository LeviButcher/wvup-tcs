import React, { useState } from 'react';
import SignInForm from '../../components/SignInForm';
import EmailOrCardSwipeForm from '../../components/EmailOrCardSwipeForm';
import useApi from '../../hooks/useApi';
import { Card } from '../../ui';

const CreateSignIn = () => {
  const [person, setPerson]: [any, any] = useState();
  const [, reasons] = useApi('reasons/active');
  const [, semesters] = useApi('reports/semesters');

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
              firstName: person.firstName,
              lastName: person.lastName,
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
            reasons={reasons || []}
            semesters={semesters || []}
          />
        </div>
      )}
    </Card>
  );
};

export default CreateSignIn;
