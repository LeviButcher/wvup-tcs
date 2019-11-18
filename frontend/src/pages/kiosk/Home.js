import React, { useEffect } from 'react';
import styled from 'styled-components';
import { Link, Header, Card, KioskFullScreenContainer } from '../../ui';

const InfoPopUp = ({ className, info }) => {
  useEffect(() => {
    setTimeout(() => {
      const infoNode = document.querySelector('#info');
      if (infoNode) {
        if (infoNode.parentNode) infoNode.parentNode.removeChild(infoNode);
      }
    }, 10000);
  }, [info]);
  return (
    <div
      className={className}
      style={{ justifySelf: 'center', gridArea: 'info' }}
      id="info"
    >
      <h4>{info}</h4>
    </div>
  );
};

const StyledInfoPopUp = styled(InfoPopUp)`
  padding: 0.5rem 1rem;
  background: #2f9cda66;
  margin: 1rem;
  border-left: 5px solid #444;
`;

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

// $FlowFixMe
const LandingPageKioskFullScreen = styled(KioskFullScreenContainer)`
  padding: 0;
  display: grid;
  grid-template:
    'info info' auto
    'boxLeft boxRight' 1fr
    'footer footer' auto / 50% 50%;
  align-items: flex-end;
  justify-content: flex-start;
  @media (max-width: 880px) {
    grid-template:
      'info' 1fr
      'boxLeft' 500px
      'boxRight' 500px
      'footer' auto;
    grid-gap: 30px;
  }
`;

type Props = {
  location: {
    state: {
      info: string
    }
  }
};

const Home = ({ location }: Props) => (
  <LandingPageKioskFullScreen>
    {location && location.state && location.state.info && (
      <StyledInfoPopUp info={location.state.info} />
    )}
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
  </LandingPageKioskFullScreen>
);

export default Home;
