import React from 'react';
import styled from 'styled-components';
import { Link } from '@reach/router';
import Header from '../../ui/Header';

const Layout = ({ children }) => (
  <LayoutGrid>
    <SideNav />
    <div>{children}</div>
  </LayoutGrid>
);

const LayoutGrid = styled.div`
  width: 100vw;
  height: 100vh;
  max-height: 100vh;
  y-overflow: none;
  display: grid;
  grid-template: 'nav main' 1fr / 300px auto;

  & > * {
    padding: ${props => props.theme.padding};
  }

  & > nav {
    height: 100%;
    padding: 0 ${props => props.theme.padding};
    background-color: #afafaf;
  }
`;

const SideNav = () => {
  return (
    <nav>
      <Header align="center">TCS</Header>
      <LinkGroup>
        <Header type="h2">Lookups</Header>
        <Link to="signins">Sign Ins</Link>
        <Link to="tours">Class Tours</Link>
      </LinkGroup>
      <LinkGroup>
        <Header type="h2">Reports</Header>
        <Link to="signins">Class Tour</Link>
        <Link to="signins">Volunteer</Link>
        <Link to="signins">Weekly Visits</Link>
        <Link to="signins">Peak Hours</Link>
        <Link to="signins">Success</Link>
      </LinkGroup>
      <LinkGroup>
        <Header type="h2">Admin</Header>
        <Link to="signins">Users</Link>
        <Link to="signins">Reason for Visiting</Link>
      </LinkGroup>
    </nav>
  );
};

const LinkGroup = styled.div`
  border-bottom 1px solid #ddd;
  text-align: right;
  padding: 10px 0;
  & > * {
    display: block;
  }
`;

export default Layout;
