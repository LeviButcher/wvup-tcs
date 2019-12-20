import React from 'react';
import SignInForm from '../../components/SignInForm';
import useApi from '../../hooks/useApi';
import { Card } from '../../ui';

type Props = {
  id: string
};

const UpdateSignIn = ({ id }: Props) => {
  const [, signIn] = useApi(`sessions/${id}`);
  const [, reasons] = useApi('reasons');
  const [, semesters] = useApi('reports/semesters');

  return (
    <>
      {signIn &&
        reasons &&
        semesters &&
        signIn.email &&
        reasons.length > 0 &&
        semesters.length > 0 && (
          <Card>
            <SignInForm
              signInRecord={{
                semesterCode: signIn.semesterCode,
                email: signIn.email,
                schedule: signIn.schedule,
                inTime: signIn.inTime,
                outTime: signIn.outTime,
                selectedReasons: signIn.selectedReasons,
                selectedClasses: signIn.selectedClasses,
                personId: signIn.personId,
                personType: signIn.personType,
                tutoring: signIn.tutoring,
                id: signIn.id,
                firstName: signIn.firstName,
                lastName: signIn.lastName
              }}
              reasons={reasons}
              semesters={semesters}
            />
          </Card>
        )}
    </>
  );
};

export default UpdateSignIn;
