import React from 'react';
import styled from 'styled-components';
import { Header, Link } from '../../ui';
import Logout from '../Logout';

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
      <Logout />
      <LinkGroup>
        <Header type="h2">Lookups</Header>
        <NavLink to="signins">Sign Ins</NavLink>
        <NavLink to="tours">Class Tours</NavLink>
      </LinkGroup>
      <LinkGroup>
        <Header type="h2">Reports</Header>
        <NavLink to="report/tours">Class Tour</NavLink>
        <NavLink to="report/volunteer">Volunteer</NavLink>
        <NavLink to="report/weekly-visits">Weekly Visits</NavLink>
        <NavLink to="report/peak-hours">Peak Hours</NavLink>
        <NavLink to="report/reason-for-visiting">Reasons For Visiting</NavLink>
        <NavLink to="report/success">Success</NavLink>
      </LinkGroup>
      <LinkGroup>
        <Header type="h2">Admin</Header>
        <NavLink to="admin/users">Users</NavLink>
        <NavLink to="admin/">Reason for Visiting</NavLink>
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
  display: block;
  color: #bbb;
  margin: 0.5rem auto;
  &:hover,
  &:focus {
    color: ${props => props.theme.color.accent};
  }
  &[aria-current='page'] {
    color: ${props => props.theme.color.accent};
  }
`;

export default Layout;
