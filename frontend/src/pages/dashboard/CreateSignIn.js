import React, { useState } from 'react';
import SignInForm from '../../components/SignInForm';
import EmailOrCardSwipeForm from '../../components/EmailOrCardSwipeForm';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';
import { Card } from '../../ui';

const CreateSignIn = () => {
  const [person, setPerson]: [any, any] = useState();
  const [, { body: reasons }] = useApiWithHeaders('reasons/active');
  const [isTeacher, setIsTeacher] = useState(false);

  return (
    <Card>
      {!person ? (
        <div>
          <h1>Lookup up a Student or Teacher</h1>
          Currently searching for a {isTeacher ? 'Teacher' : 'Student'}
          <button
            onClick={() => setIsTeacher(currState => !currState)}
            type="button"
          >
            Change to {isTeacher ? 'Student' : 'Teacher'} lookup
          </button>
          <EmailOrCardSwipeForm
            teacher={isTeacher}
            afterValidSubmit={value => Promise.resolve(setPerson(value))}
          />
        </div>
      ) : (
        <div>
          <SignInForm
            signInRecord={{
              semesterId: person.semesterId,
              email: person.studentEmail || person.teacherEmail,
              classSchedule: person.classSchedule,
              inTime: '',
              outTime: '',
              selectedReasons: [],
              selectedClasses: [],
              personId: person.studentId || person.teacherId,
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
