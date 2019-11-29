import React from 'react';
// eslint-disable-next-line import/no-extraneous-dependencies
import { render } from '@testing-library/react';
import { ThemeProvider } from 'styled-components';
import Theme from '../theme.json';

const AllTheProviders = ({ children }) => {
  return <ThemeProvider theme={Theme}>{children}</ThemeProvider>;
};

// $FlowFixMe
const customRender = (ui, options) =>
  // $FlowFixMe
  render(ui, { wrapper: AllTheProviders, ...options });

// re-export everything
export * from '@testing-library/react';

// override render method
export { customRender as render };
