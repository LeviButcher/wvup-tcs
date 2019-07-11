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

const mockSuccess = {};
const mockFetchPromise = Promise.resolve({
  // 3
  json: () => Promise.resolve(mockSuccess)
});
jest.spyOn(global, 'fetch').mockImplementation(() => mockFetchPromise);

test('Invalid Email displays error', async () => {
  const {
    getByLabelText,
    getByText,
    findByLabelText,
    getById,
    findByText
  } = render(<SignIn />);

  // Wait for page to update with query text
  const emailInput = getByLabelText(/email/i);
  const email = 'MyFakeEmail@yahoo.com';

  fireEvent.change(emailInput, { target: { value: email } });
  fireEvent.click(getByText(/submit/i));

  expect(emailInput.value).toEqual(email);
  const emailError = await waitForElement(() => findByText(/address/i));
  expect(emailError).toBeDefined();
});
