import React from 'react';
import ScaleLoader from 'react-spinners/ScaleLoader';
import UserForm from '../../components/UserForm';
import useApi from '../../hooks/useApi';

type Props = {
  id: string
};

const UpdateUser = ({ id }: Props) => {
  const [loading, user] = useApi(`users/${id}`);
  return (
    <div>
      <ScaleLoader loading={loading} />
      {/* // $FlowFixMe */}
      {!loading && <UserForm user={user} />}
    </div>
  );
};

export default UpdateUser;
