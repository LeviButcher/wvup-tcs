import React from 'react';
import type { Node } from 'react';
import styled from 'styled-components';
import Header from '../../ui/Header';

type Props = {
  children: Node
};

const Layout = ({ children }: Props) => (
  <div>
    <StyledHeader>Welcome to the Tutoring Center</StyledHeader>
    <div>{children}</div>
  </div>
);

const StyledHeader = styled(Header)`
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: ${props => props.theme.color.main};
  color: ${props => props.theme.color.accent};
  height: ${props => props.theme.kioskHeaderSize};
`;

export default Layout;
