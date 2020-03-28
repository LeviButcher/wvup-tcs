/* eslint-disable no-undef */
import React from 'react';
import UploadSignIns from './UploadSignIns';
import {
  render,
  waitForElement,
  getByTestId,
  fireEvent
} from '../../test-utils/CustomReactTestingLibrary';

const backendURL = process.env.REACT_APP_BACKEND || '';

test('Renders Successfully', () => {
  const { container } = render(<UploadSignIns />);
  expect(container).not.toBeNull();
});

test('Can Not Submit Until file is selected', () => {
  const { getByLabelText, getByText } = render(<UploadSignIns />);
  const file = new File(['csv data goes here'], 'csvFile.csv', {
    type: 'text/csv'
  });
  const submit = getByText(/submit/i);
  const fileInput = getByLabelText(/File/i);

  expect(submit).toBeDisabled();
  fireEvent.change(fileInput, { target: { files: [file] } });
  expect(submit).not.toBeDisabled();
});

test('Submit file and call correct backend endpoint', () => {
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      status: 200,
      json: () => Promise.resolve({})
    })
  );

  global.fetch = fakeFetch;

  const { getByLabelText, getByText } = render(<UploadSignIns />);
  const file = new File(['csv data goes here'], 'csvFile.csv', {
    type: 'text/csv'
  });
  const submit = getByText(/submit/i);
  const fileInput = getByLabelText(/File/i);

  fireEvent.change(fileInput, { target: { files: [file] } });
  fireEvent.submit(submit);

  expect(fakeFetch).toHaveBeenCalledTimes(1);
  expect(fakeFetch).toHaveBeenCalledWith(`${backendURL}sessions/upload`, {
    method: 'POST',
    headers: {
      Authorization: 'Bearer '
    },
    body: expect.anything()
  });
});

test('Submit file but api responds with non 200 status code, should display errors', async () => {
  const message = "Person with email: 'lbutche3@wvup.edu' does not exist";
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      status: 500,
      json: () =>
        Promise.resolve({
          message
        })
    })
  );

  global.fetch = fakeFetch;

  const { getByLabelText, getByText, findByText } = render(<UploadSignIns />);
  const file = new File(['csv data goes here'], 'csvFile.csv', {
    type: 'text/csv'
  });
  const submit = getByText(/submit/i);
  const fileInput = getByLabelText(/File/i);

  fireEvent.change(fileInput, { target: { files: [file] } });
  fireEvent.submit(submit);

  const error = await findByText(message);
  expect(error).not.toBeNull();
});
