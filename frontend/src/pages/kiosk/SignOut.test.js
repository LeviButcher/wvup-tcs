import React from 'react';
import {
  render,
  fireEvent,
  cleanup,
  wait,
  waitForElement
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
  expect(getByText(/Submit/)).toBeDisabled();
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

test('Should send fetch call with expected body when using valid email address', async () => {
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      headers: [],
      json: () => Promise.resolve({ id: '98' }),
      status: 200
    })
  );
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
    expect(fakeFetch).toHaveBeenCalledTimes(2);
    expect(fakeFetch).toHaveBeenCalledWith(`${backendURL}session/signout`, {
      headers: {
        'Content-Type': 'application/json'
      },
      method: 'PUT',
      body: JSON.stringify({ id: '98' })
    });
  });
});

test('Card swipe calls fetch with correct arguments', async () => {
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      headers: [],
      json: () => Promise.resolve({ id: '98' }),
      status: 200
    })
  );
  global.fetch = fakeFetch;

  const { container } = render(<SignOut />);

  // Format of wvup id card: %{startofEmail}?;{wvupId}?
  const card = ['%', 'l', 'b', '?', ';', '9', '8', '?', '\n'];

  card.forEach(char => fireEvent.keyPress(container, toKeyCode(char)));

  await wait(() => {
    expect(fakeFetch).toHaveBeenCalledTimes(2);
    expect(fakeFetch).toHaveBeenCalledWith(`${backendURL}session/signout`, {
      headers: {
        'Content-Type': 'application/json'
      },
      method: 'PUT',
      body: JSON.stringify({ id: '98' })
    });
  });
});

test('Should display fetch error message when fetch returns non 2XX status code', async () => {
  const email = 'fake@wvup.edu';
  const error = { message: 'You are not signed in!' };
  const fakeFetch = jest.fn(url => {
    switch (url) {
      case `${backendURL}person/${email}`:
        return Promise.resolve({
          headers: [],
          json: () => Promise.resolve({ id: '98' }),
          status: 200
        });
      case `${backendURL}session/signout`:
        return Promise.resolve({
          status: 400,
          json: () => Promise.resolve(error)
        });
      default:
        return Promise.reject(new Error('Something went wrong in test'));
    }
  });
  global.fetch = fakeFetch;

  const { getByText, getByLabelText } = render(<SignOut />);

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: email }
  });

  await wait(() => {
    expect(getByText(/submit/i)).not.toBeDisabled();
  });

  fireEvent.submit(getByText(/submit/i));

  const errorMessage = await waitForElement(() => getByText(error.message));
  expect(errorMessage).toBeDefined();
});

test('Should display fetch error message when fetch returns non 2XX with card swipe', async () => {
  const id = '98';
  const error = { message: 'You are not signed in!' };
  const fakeFetch = jest.fn(url => {
    switch (url) {
      case `${backendURL}person/${id}`:
        return Promise.resolve({
          headers: [],
          json: () => Promise.resolve({ id: '98' }),
          status: 200
        });
      case `${backendURL}session/signout`:
        return Promise.resolve({
          status: 400,
          json: () => Promise.resolve(error)
        });
      default:
        return Promise.reject(new Error('Something went wrong in test'));
    }
  });
  global.fetch = fakeFetch;

  const { container, getByText } = render(<SignOut />);

  // Format of wvup id card: %{startofEmail}?;{wvupId}?
  const card = ['%', 'l', 'b', '?', ';', '9', '8', '?', '\n'];

  card.forEach(char => fireEvent.keyPress(container, toKeyCode(char)));

  const errorMessage = await waitForElement(() => getByText(error.message));
  expect(errorMessage).toBeDefined();
});
