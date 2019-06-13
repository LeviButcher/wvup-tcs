import React from 'react';
import styled from 'styled-components';
import Card from '../../ui/Card';

const Home = () => (
  <FullScreenContainer>
    <StyledCard styled={{ 'font-size': '75px' }}>SignIn</StyledCard>
    <StyledCard>SignOut</StyledCard>
  </FullScreenContainer>
);

const StyledCard = styled(Card)`
  padding: 4rem;
  box-shadow: 0 0 5px 1px;
`;

const FullScreenContainer = styled.div`
  height: calc(100vh - 75px);
  display: flex;
  align-items: center;
  justify-content: space-evenly;
`;

export default Home;
