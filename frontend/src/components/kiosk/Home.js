import React from 'react';
import styled from 'styled-components';
import { Link, Header, Card } from '../../ui';

const Home = () => (
  <FullScreenContainer>
    <BoxLink to="/signin" style={{ 'grid-area': 'boxLeft' }}>
      <Box>Sign In</Box>
    </BoxLink>
    <BoxLink to="/signout" style={{ 'grid-area': 'boxRight' }}>
      <Box>Sign Out</Box>
    </BoxLink>
    <Footer>
      <Header type="h3" align="center">
        <Link to="/signin/teacher">Sign in for teachers</Link>
      </Header>
    </Footer>
  </FullScreenContainer>
);

const BoxLink = styled(Link)`
  align-self: center;
  justify-self: center;

  &:hover {
    transform: scale(1.1);
  }
`;

const Footer = styled.footer`
  background-color: #afafaf;
  grid-area: footer;
  padding: 0 ${props => props.theme.padding};
`;

const Box = styled(Card)`
  padding: 0;
  width: 400px;
  height: 400px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 60px;
  align-self: center;
  justify-self: center;
`;

const FullScreenContainer = styled.div`
  height: calc(100vh - ${props => props.theme.kioskHeaderSize});
  width: 100%;
  display: grid;
  grid-template:
    'boxLeft boxRight' 1fr
    'footer footer' auto / 50% 50%;
  align-items: flex-end;
  @media (max-width: 880px) {
    grid-template:
      'boxLeft' 500px
      'boxRight' 500px
      'footer' auto;
    grid-gap: 30px;
  }
`;

export default Home;
