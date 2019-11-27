import React, { useState } from 'react';
import SignInForm from '../../components/SignInForm';
import EmailOrCardSwipeForm from '../../components/EmailOrCardSwipeForm';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';

const CreateSignIn = () => {
  const [person, setPerson]: [any, any] = useState();
  const [, { body: reasons }] = useApiWithHeaders('reasons/active');

  // Need to make getting student information send back personType
  return (
    <div>
      {!person ? (
        <EmailOrCardSwipeForm
          // $FlowFixMe
          afterValidSubmit={value => Promise.resolve(setPerson(value))}
        />
      ) : (
        <div>
          <SignInForm
            signInRecord={{
              semesterId: person.semesterId,
              email: person.studentEmail,
              classSchedule: person.classSchedule,
              inTime: '',
              outTime: '',
              selectedReasons: [],
              selectedClasses: [],
              personId: person.studentId,
              personType: person.personType,
              tutoring: false
            }}
            reasons={reasons}
          />
        </div>
      )}
    </div>
  );
};

export default CreateSignIn;
