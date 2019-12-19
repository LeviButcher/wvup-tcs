import React from 'react';
import SignInForm from '../../components/SignInForm';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';
import { Card } from '../../ui';

type Props = {
  id: string
};

const UpdateSignIn = ({ id }: Props) => {
  const [, { body: signIn }]: [boolean, { body: any }, any] = useApiWithHeaders(
    `sessions/${id}`
  );
  const [, { body: reasons }] = useApiWithHeaders('reasons');
  const [, { body: semesters }] = useApiWithHeaders('reports/semesters');

  return (
    <>
      {signIn && signIn.email && reasons.length > 0 && semesters.length > 0 && (
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
