import React from 'react';
import ScaleLoader from 'react-spinners/ScaleLoader';
import ClassTourForm from '../../components/ClassTourForm';
import useApi from '../../hooks/useApiWithHeaders';

type Props = {
  id: string
};

const UpdateClassTour = ({ id }: Props) => {
  const [loading, { body }] = useApi(`classtours/${id}`);
  return (
    <div style={{ margin: 'auto' }}>
      {/* $FlowFixMe */}
      {loading ? <ScaleLoader /> : <ClassTourForm classTour={body} />}
    </div>
  );
};

export default UpdateClassTour;
