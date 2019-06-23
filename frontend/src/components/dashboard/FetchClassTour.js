import React from 'react';

const fakeTour = {
  name: 'Faky',
  date: new Date(),
  count: '20'
};

const FetchClassTour = ({ Component, classTourId }) => {
  return (
    <>
      <Component classTour={fakeTour} action="Update" />
    </>
  );
};

export default FetchClassTour;
