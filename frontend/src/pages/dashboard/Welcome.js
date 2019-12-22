import React from 'react';
import ScaleLoader from 'react-spinners/ScaleLoader';
import styled from 'styled-components';
import { Header } from '../../ui';
import SignInsTable from '../../components/SignInsTable';
import useApi from '../../hooks/useApi';

const BigText = styled.span`
  font-size: 5em;
`;

const Welcome = () => {
  const today = new Date().toLocaleDateString();
  const [loading, todaySessions, error] = useApi(
    `sessions?start=${today}&end=${today}`
  );

  return (
    <div>
      <CustomHeader align="center">Tutoring Center System</CustomHeader>
      <ScaleLoader sizeUnit="px" size={150} loading={loading} align="center" />
      {!loading && todaySessions && (
        <>
          <Header align="center" type="h2">
            <BigText>{todaySessions.totalRecords}</BigText>{' '}
            {todaySessions.totalRecords !== 1 ? 'Students' : 'Student'} Helped
            Today
          </Header>
          {todaySessions.data && todaySessions.data.length >= 1 && (
            <div>
              <h4>Most Recent Visitors</h4>
              <SignInsTable signIns={todaySessions.data} />
            </div>
          )}
          {error && <div>{error.message}</div>}
        </>
      )}
    </div>
  );
};

const CustomHeader = styled(Header)`
  margin: auto;
  border-bottom: 2px solid ${props => props.theme.color.main};
  margin-bottom: 3rem;
`;

export default Welcome;
