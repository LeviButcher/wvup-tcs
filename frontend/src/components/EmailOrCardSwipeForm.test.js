import React from 'react';
import '@testing-library/jest-dom/extend-expect';
import {
  render,
  fireEvent,
  cleanup,
  wait
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
  const mockSubmit = jest.fn();
  const { getByText, getByLabelText } = render(
    <EmailOrCardSwipeForm afterValidSubmit={mockSubmit} teacher={false} />
  );

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: 'lbutche3@wvup.edu' }
  });
  fireEvent.submit(getByText(/submit/i));

  await wait(() => {
    expect(mockSubmit).toHaveBeenCalledTimes(1);
  });
});
