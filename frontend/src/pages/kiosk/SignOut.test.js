import React from 'react';
import '@testing-library/jest-dom/extend-expect';
import {
  render,
  fireEvent,
  cleanup,
  wait
} from '../../test-utils/CustomReactTestingLibrary';
import SignOut from './SignOut';
import toKeyCode from '../../test-utils/toKeyCode';

const backendURL = process.env.REACT_APP_BACKEND || '';

test('Renders with required props', () => {
  const { container } = render(<SignOut />);
  expect(container).toBeDefined();
});

test('Expect submit to be disabled on render', () => {
  const { getByText } = render(<SignOut />);
  const submit = getByText(/Submit/);
  expect(submit).toBeDisabled();
});

test("Can't submit with non wvup.edu email", async () => {
  const { getByText, getByLabelText } = render(<SignOut />);

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: 'fake@gmail.com' }
  });

  await wait(() => {
    const submit = getByText(/Submit/);
    expect(submit).toBeDisabled();
  });
});

test("Can't submit with non wvup.edu email, change email to valid allows submission", async () => {
  const { getByText, getByLabelText, queryByText } = render(<SignOut />);

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: 'fake@gmail.com' }
  });
  fireEvent.blur(getByLabelText(/email/i));

  await wait(() => {
    const submit = getByText(/Submit/);
    expect(submit).toBeDisabled();
    expect(getByText(/Must be a WVUP/i)).toBeDefined();
  });

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: 'fake@wvup.edu' }
  });
  fireEvent.blur(getByLabelText(/email/i));

  await wait(() => {
    const submit = getByText(/Submit/);
    expect(submit).not.toBeDisabled();
    expect(queryByText(/Must be a WVUP/i)).toBeNull();
  });
});

test('Submit with valid email sends fetch call to api with email', async () => {
  const fakeFetch = jest.fn(() => Promise.resolve({}));
  global.fetch = fakeFetch;

  const { getByText, getByLabelText } = render(<SignOut />);

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: 'fake@wvup.edu' }
  });

  await wait(() => {
    expect(getByText(/submit/i)).not.toBeDisabled();
  });

  fireEvent.submit(getByText(/submit/i));

  await wait(() => {
    expect(fakeFetch).toHaveBeenCalledTimes(1);
    expect(fakeFetch).toHaveBeenCalledWith(
      `${backendURL}signins/fake@wvup.edu/signout`,
      {
        headers: {
          Authorization: 'Bearer null',
          'Content-Type': 'application/json'
        },
        method: 'PUT'
      }
    );
  });
});

test('Card swipe calls fetch with correct arguments', async () => {
  const fakeFetch = jest.fn(() => Promise.resolve({}));
  global.fetch = fakeFetch;

  const { container } = render(<SignOut />);

  // Format of wvup id card: %{startofEmail}?;{wvupId}?
  const card = ['%', 'l', 'b', '?', ';', '9', '8', '?', '\n'];

  card.forEach(char => fireEvent.keyPress(container, toKeyCode(char)));

  await wait(() => {
    expect(fakeFetch).toHaveBeenCalledTimes(1);
    expect(fakeFetch).toHaveBeenCalledWith(`${backendURL}signins/98/signout`, {
      headers: {
        Authorization: 'Bearer null',
        'Content-Type': 'application/json'
      },
      method: 'PUT'
    });
  });
});
