import React from 'react';
import { navigate } from '@reach/router';
import styled from 'styled-components';
import { Header } from '../ui';

const token = process.env.REACT_APP_TOKEN || '';

const clearSession = () => {
  localStorage.removeItem(token);
  navigate('/login');
};

const getUserName = () => localStorage.getItem('username');

const Logout = () => {
  return (
    <Header align="center" type="h5">
      {getUserName()} - <SpecialText onClick={clearSession}>Logout</SpecialText>
    </Header>
  );
};

const SpecialText = styled.span`
  color: ${props => props.theme.color.warning};
  font-weight: bold;
  &:hover {
    color: ${props => props.theme.color.accent};
    cursor: pointer;
  }
`;

export default Logout;
