import React from 'react';
import ScaleLoader from 'react-spinners/ScaleLoader';
import ClassTourForm from '../../components/ClassTourForm';
import useApi from '../../hooks/useApi';

type Props = {
  id: string
};

const UpdateClassTour = ({ id }: Props) => {
  const [loading, classTour] = useApi(`classtours/${id}`);

  return (
    <div style={{ margin: 'auto' }}>
      {!loading && classTour ? (
        <ClassTourForm classTour={classTour} />
      ) : (
        <ScaleLoader loading={loading} />
      )}
    </div>
  );
};

export default UpdateClassTour;
