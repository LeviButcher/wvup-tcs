/* eslint-disable */
// test-utils.js
import React from 'react';
import { render } from '@testing-library/react';
import Theme from '../theme.json';
import { ThemeProvider } from 'styled-components';

const AllTheProviders = ({ children }) => {
  return <ThemeProvider theme={Theme}>{children}</ThemeProvider>;
};

const customRender = (ui, options) =>
  render(ui, { wrapper: AllTheProviders, ...options });

// re-export everything
export * from '@testing-library/react';

// override render method
export { customRender as render };
