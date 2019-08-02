import React from 'react';

import ScaleLoader from 'react-spinners/ScaleLoader';

const LoadingContent = ({ loading, children, data }) => (
  <>
    <ScaleLoader sizeUnit="px" size={150} loading={loading} align="center" />
    {!loading && (
      <>
        {data.body && data.body.length >= 1 && children}
        {data.body && data.body.length < 1 && <h3>No content was returned</h3>}
      </>
    )}
  </>
);
export default LoadingContent;
