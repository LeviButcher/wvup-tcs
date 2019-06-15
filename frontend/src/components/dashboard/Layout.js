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
  grid-template: 'nav main' 1fr / 350px auto;

  & > nav {
    height: 100%;
    background-color: #afafaf;
  }
`;

const SideNav = () => {
  return (
    <nav>
      <Header text="TCS" align="center" />
      <LinkGroup>
        <Header text="Visits" type="h2" />
        <Link to="signins">Sign Ins</Link>
        <Link to="tours">Class Tours</Link>
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
