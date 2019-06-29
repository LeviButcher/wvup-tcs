import React from 'react';
import styled from 'styled-components';
import { Header, Link } from '../../ui';

const Layout = ({ children }) => (
  <LayoutGrid>
    <SideNav />
    <main>{children}</main>
  </LayoutGrid>
);

const LayoutGrid = styled.div`
  height: 100vh;
  max-height: 100vh;
  overflow-y: hidden;
  display: grid;
  grid-template: 'nav main' 100vh / 250px auto;


  & > main {
    padding: ${props => props.theme.padding};
    overflow-y: scroll;
  }

  & > nav {
    height: 100%;
    padding: 0 ${props => props.theme.padding};
    background-color: ${props => props.theme.color.main};
    color: #fff;
  }
`;

const SideNav = () => {
  return (
    <nav>
      <MainHeader align="center">TCS</MainHeader>
      <LinkGroup>
        <Header type="h2">Lookups</Header>
        <NavLink to="signins">Sign Ins</NavLink>
        <NavLink to="tours">Class Tours</NavLink>
      </LinkGroup>
      <LinkGroup>
        <Header type="h2">Reports</Header>
        <NavLink to="signins">Class Tour</NavLink>
        <NavLink to="signins">Volunteer</NavLink>
        <NavLink to="signins">Weekly Visits</NavLink>
        <NavLink to="signins">Peak Hours</NavLink>
        <NavLink to="signins">Success</NavLink>
      </LinkGroup>
      <LinkGroup>
        <Header type="h2">Admin</Header>
        <NavLink to="signins">Users</NavLink>
        <NavLink to="signins">Reason for Visiting</NavLink>
      </LinkGroup>
    </nav>
  );
};

const MainHeader = styled(Header)`
  color: ${props => props.theme.color.accent};
`;

const LinkGroup = styled.div`
  text-align: right;
  padding: 10px 0;
  & > * {
    display: block;
  }
`;

const NavLink = styled(Link)`
  color: #bbb;
  margin: 0.5rem auto;
  &:hover {
    color: ${props => props.theme.color.accent};
  }
`;

export default Layout;
