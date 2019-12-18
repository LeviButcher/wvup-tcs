import React from 'react';
import SignInForm from '../../components/SignInForm';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';
import { Card } from '../../ui';

type Props = {
  id: string
};

const UpdateSignIn = ({ id }: Props) => {
  const [, { body: signIn }]: [boolean, { body: any }, any] = useApiWithHeaders(
    `signins/${id}`
  );
  const [, { body: reasons }] = useApiWithHeaders('reasons/active');
  const [, { body: semesters }] = useApiWithHeaders('reports/semesters');

  // Doesn't work because api doesn't give everything needed
  return (
    <>
      {signIn && (
        <Card>
          <SignInForm
            signInRecord={{
              semesterId: signIn.semesterId,
              email: signIn.email,
              classSchedule: signIn.courses,
              inTime: signIn.inTime,
              outTime: signIn.outTime,
              selectedReasons: signIn.reasons,
              selectedClasses: signIn.courses,
              personId: signIn.personId,
              personType: signIn.personType,
              tutoring: signIn.tutoring,
              id: signIn.id
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
