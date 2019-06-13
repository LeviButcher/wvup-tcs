import React from 'react';
import styled from 'styled-components';
import Header from '../../ui/Header';

const Layout = ({ children }) => (
  <div>
    <StyledHeader text="Welcome to the Tutoring Center" />
    <div>{children}</div>
  </div>
);

const StyledHeader = styled(Header)`
  text-align: center;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: rgb(9, 18, 38);
  color: #ffc814;
  height: 75px;
`;

export default Layout;
