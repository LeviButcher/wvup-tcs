import React from 'react';
import ScaleLoader from 'react-spinners/ScaleLoader';
import ReasonForm from '../../components/ReasonForm';
import useApi from '../../hooks/useApi';

type Props = {
  id: string
};

const UpdateReason = ({ id }: Props) => {
  const [loading, reason] = useApi(`reasons/${id}`);
  return (
    <div style={{ margin: 'auto' }}>
      {/* $FlowFixMe */}
      {loading ? <ScaleLoader /> : <ReasonForm reason={reason} />}
    </div>
  );
};

export default UpdateReason;
