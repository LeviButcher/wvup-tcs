/* eslint-disable */
import React from 'react';
import '@testing-library/jest-dom/extend-expect';
import {
  render,
  fireEvent,
  cleanup,
  waitForElementToBeRemoved
} from '../test-utils/CustomReactTestingLibrary';
import EmailOrCardSwipeForm from './EmailOrCardSwipeForm';

const mockSuccessResponse = {};
const mockJsonPromise = Promise.resolve(mockSuccessResponse); // 2
const mockFetchPromise = Promise.resolve({
  json: () => mockJsonPromise,
  status: 200
});

jest.spyOn(global, 'fetch').mockImplementation(() => mockFetchPromise);

test('Form renders successfully', async () => {
  render(<EmailOrCardSwipeForm afterValidSubmit={jest.fn()} teacher={false} />);
});

test('Form can be submitted with wvup.edu address', async () => {
  const mockSubmit = jest.fn(x => x);
  const { findByLabelText, findByText, debug, queryByText } = render(
    <EmailOrCardSwipeForm afterValidSubmit={mockSubmit} teacher={false} />
  );
  const emailInput = await findByLabelText(/Email/);
  fireEvent.change(emailInput, {
    target: { value: 'lbutche3@wvup.edu' }
  });
  fireEvent.click(await findByText(/Submit/));
  await waitForElementToBeRemoved(() => queryByText(/Getting information/));
  expect(mockSubmit.mock.calls.length).toBe(1);
});
