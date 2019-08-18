import React from 'react';
import styled from 'styled-components';
import { Link, Header, Card } from '../../ui';

const Home = ({ location }) => (
  <FullScreenContainer>
    {location.state.info && <InfoPopUp info={location.state.info} />}
    <BoxLink to="/signin" style={{ gridArea: 'boxLeft' }}>
      <Box>Sign In</Box>
    </BoxLink>
    <BoxLink to="/signout" style={{ gridArea: 'boxRight' }}>
      <Box>Sign Out</Box>
    </BoxLink>
    <Footer>
      <Header type="h3" align="center">
        <Link to="/signin/teacher">Sign in for teachers</Link>
      </Header>
    </Footer>
  </FullScreenContainer>
);

const InfoPopUp = ({ info }) => {
  return (
    <div style={{ justifySelf: 'center', gridArea: 'info' }}>
      <h4>{info}</h4>
    </div>
  );
};

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
    'info info' auto
    'boxLeft boxRight' 1fr
    'footer footer' auto / 50% 50%;
  align-items: flex-end;
  @media (max-width: 880px) {
    grid-template:
      'info' 1fr
      'boxLeft' 500px
      'boxRight' 500px
      'footer' auto;
    grid-gap: 30px;
  }
`;

export default Home;
