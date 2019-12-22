import React from 'react';
import type { Node } from 'react';

import ScaleLoader from 'react-spinners/ScaleLoader';

type Props = {
  loading: boolean,
  children: Node,
  data: any
};

const LoadingContent = ({ loading, children, data }: Props) => (
  <>
    <ScaleLoader sizeUnit="px" size={150} loading={loading} align="center" />
    {!loading && (
      <>
        {data && data.data.length >= 1 && children}
        {data && data.data.length < 1 && <h3>No content was returned</h3>}
      </>
    )}
  </>
);
export default LoadingContent;
