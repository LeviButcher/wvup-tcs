import React from 'react';
import styled from 'styled-components';
import { Link } from '@reach/router';
import Header from '../../ui/Header';

const Home = () => (
  <FullScreenContainer>
    <Box style={{ 'grid-area': 'boxLeft' }}>
      <Link to="/signin">SignIn</Link>
    </Box>
    <Box style={{ 'grid-area': 'boxRight' }}>
      <Link to="/signout">SignOut</Link>
    </Box>
    <Footer>
      <Header type="h3" align="center">
        <Link to="/signin/teacher">Sign in for teachers</Link>
      </Header>
    </Footer>
  </FullScreenContainer>
);

const Footer = styled.footer`
  background-color: #afafaf;
  grid-area: footer;
  padding: 0 ${props => props.theme.padding};
`;

const Box = styled.div`
  width: 400px;
  height: 400px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 0 5px 1px;
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
`;

export default Home;
