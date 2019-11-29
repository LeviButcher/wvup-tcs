import React from 'react';
import ScaleLoader from 'react-spinners/ScaleLoader';
import ReasonForm from '../../components/ReasonForm';
import useApi from '../../hooks/useApiWithHeaders';

type Props = {
  id: string
};

const UpdateReason = ({ id }: Props) => {
  const [loading, { body }] = useApi(`reasons/${id}`);
  return (
    <div style={{ margin: 'auto' }}>
      {/* $FlowFixMe */}
      {loading ? <ScaleLoader /> : <ReasonForm reason={body} />}
    </div>
  );
};

export default UpdateReason;
