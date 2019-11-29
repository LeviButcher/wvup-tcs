import React from 'react';
import ScaleLoader from 'react-spinners/ScaleLoader';
import UserForm from '../../components/UserForm';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';

type Props = {
  id: string
};

const UpdateUser = ({ id }: Props) => {
  const [loading, data] = useApiWithHeaders(`users/${id}`);
  return (
    <div>
      <ScaleLoader loading={loading} />
      {/* // $FlowFixMe */}
      {!loading && <UserForm user={data.body} />}
    </div>
  );
};

export default UpdateUser;
