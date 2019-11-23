import React from 'react';
import {
  render,
  fireEvent,
  wait
} from '../../test-utils/CustomReactTestingLibrary';
import Login from './Login';

const backendURL = process.env.REACT_APP_BACKEND || '';
const token = process.env.REACT_APP_TOKEN || '';

test('Renders with required props', () => {
  const { container } = render(<Login />);
  expect(container).toBeDefined();
});

test('Should have submit button disabled on render', () => {
  const { getByText } = render(<Login />);
  expect(getByText(/submit/i)).toBeDisabled();
});

describe('Login Process', () => {
  test('Should call backend api with username and password when submitted', async () => {
    const authToken = 'THIS IS A FAKE TOKEN TO CHECK AGAINST';
    const fakeFetch = jest.fn(() =>
      Promise.resolve({
        status: 200,
        json: () => Promise.resolve({ token: authToken, username: 'Lucy' })
      })
    );
    const fakeBack = jest.fn();
    global.fetch = fakeFetch;
    global.history.back = fakeBack;

    const { getByLabelText, getByText } = render(<Login />);

    fireEvent.change(getByLabelText(/username/i), {
      target: { value: 'test' }
    });
    fireEvent.blur(getByLabelText(/username/i));
    fireEvent.change(getByLabelText(/password/i), {
      target: { value: 'password' }
    });
    fireEvent.blur(getByLabelText(/password/i));
    fireEvent.submit(getByText(/submit/i));

    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(
        `${backendURL}users/authenticate`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({
            username: 'test',
            password: 'password'
          })
        }
      );
    });

    // After signin, returned token should be in local storage
    const storedToken = localStorage.getItem(token);
    expect(storedToken).toBe(authToken);
    const storedUserName = localStorage.getItem('username');
    expect(storedUserName).toBe('Lucy');

    expect(fakeBack).toHaveBeenCalledTimes(1);
  });

  test('Should display error whenever fetch call returns back non 2xx status code', async () => {
    const errorMessage = 'Username or password is incorrect';
    const fakeFetch = jest.fn(() =>
      Promise.resolve({
        status: 400,
        json: () => Promise.resolve({ message: errorMessage })
      })
    );
    global.fetch = fakeFetch;

    const { getByLabelText, getByText, findByText } = render(<Login />);

    fireEvent.change(getByLabelText(/username/i), {
      target: { value: 'test' }
    });
    fireEvent.blur(getByLabelText(/username/i));
    fireEvent.change(getByLabelText(/password/i), {
      target: { value: 'password' }
    });
    fireEvent.blur(getByLabelText(/password/i));
    fireEvent.submit(getByText(/submit/i));

    const displayedError = await findByText(errorMessage);
    expect(displayedError).toBeDefined();
  });
});
