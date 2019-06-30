import React, { useEffect, useState } from 'react';
import callApi from '../../utils/callApi';

const FetchClassTour = ({ Component, classTourId, ...props }) => {
  const [tour, setTour] = useState();
  useEffect(() => {
    callApi(
      `${process.env.REACT_APP_BACKEND}classtours/${classTourId}`,
      'GET',
      null
    )
      .then(async res => {
        setTour(await res.json());
      })
      .catch(e => {
        alert(e);
      });
  }, [classTourId]);

  return <>{tour && <Component classTour={tour} classTourId {...props} />}</>;
};

export default FetchClassTour;
