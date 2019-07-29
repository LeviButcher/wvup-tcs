import React from 'react';
import ScaleLoader from 'react-spinners/ScaleLoader';
import styled from 'styled-components';
import { Header } from '../../ui';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';
import SignInsTable from '../SignInsTable';

const BigText = styled.span`
  font-size: 5em;
`;

const Welcome = () => {
  const [loading, data, errors] = useApiWithHeaders('lookups/daily');
  const totalVisits = (data && data.headers['total-records']) || 0;
  console.log(data);
  return (
    <div>
      <CustomHeader align="center">Tutoring Center System</CustomHeader>
      <ScaleLoader sizeUnit="px" size={150} loading={loading} />
      {!loading && (
        <>
          <Header align="center" type="h2">
            <BigText>{totalVisits}</BigText>{' '}
            {totalVisits > 1 ? 'Students' : 'Student'} Helped Today
          </Header>
          <div>
            <h4>Most Recent Visitors</h4>
            <SignInsTable signIns={data.body} />
          </div>
        </>
      )}
      {errors && <div>{errors}</div>}
    </div>
  );
};

const CustomHeader = styled(Header)`
  margin: auto;
  border-bottom: 2px solid ${props => props.theme.color.main};
  margin-bottom: 3rem;
`;

export default Welcome;
