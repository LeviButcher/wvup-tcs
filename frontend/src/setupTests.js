/* eslint-disable import/no-extraneous-dependencies */
/* eslint-disable import/no-unresolved */
// react-testing-library renders your components to document.body,
// this will ensure they're removed after each test.
// $FlowFixMe
import '@testing-library/react/cleanup-after-each';

// this adds jest-dom's custom assertions
import '@testing-library/jest-dom/extend-expect';
