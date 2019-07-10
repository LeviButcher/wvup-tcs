/* eslint-disable */
import React from 'react';
import {
  render,
  fireEvent,
  cleanup,
  waitForElement
} from 'CustomReactTestingLibrary'; // eslint-disable-line
import SignIn from './SignIn';

// put this in jest config
afterEach(cleanup);

test('Valid Email loads in class list', async () => {
  const { getByLabelText, getById } = render(<SignIn />);

  // Wait for page to update with query text
  const emailInput = getByLabelText(/emai/i);
  const email = 'lbutche3@wvup.edu';

  fireEvent.change(emailInput, { target: { value: email } });

  expect(emailInput.value).toEqual(email);
  const classList = await waitForElement(() => getByLabelText('courses'));
});
