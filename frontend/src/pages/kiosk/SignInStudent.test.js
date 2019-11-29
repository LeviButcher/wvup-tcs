import React from 'react';
import {
  render,
  fireEvent,
  wait
} from '../../test-utils/CustomReactTestingLibrary';
import SignInStudent from './SignInStudent';

const backendURL = process.env.REACT_APP_BACKEND || '';

test('Renders with required props', () => {
  const { container } = render(<SignInStudent />);
  expect(container).toBeDefined();
});

test('Submit button disabled on mount', () => {
  const { getByText } = render(<SignInStudent />);
  expect(getByText(/submit/i)).toBeDisabled();
});

test("Can't submit with non-wvup email, can submit with wvup email once changed", async () => {
  const { getByText, getByLabelText, findByText, queryByText } = render(
    <SignInStudent />
  );

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: 'test@gmail.com' }
  });
  fireEvent.blur(getByLabelText(/email/i));

  const emailError = await findByText(/wvup email/i);
  expect(emailError).toBeDefined();
  expect(getByText(/submit/i)).toBeDisabled();

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: 'test@wvup.edu' }
  });
  fireEvent.blur(getByLabelText(/email/i));

  await wait(() => {
    expect(queryByText(/wvup email/i)).toBeNull();
    expect(getByText(/submit/i)).not.toBeDisabled();
  });
});

test('Enter valid email and submit, fetch call to backend returns bad request, display error message', async () => {
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      status: 400,
      json: () =>
        Promise.resolve({ message: "Can't find email, please try again" })
    })
  );
  global.fetch = fakeFetch;

  const { getByText, getByLabelText, findByText } = render(<SignInStudent />);

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: 'test@wvup.edu' }
  });
  fireEvent.blur(getByLabelText(/email/i));

  fireEvent.submit(getByText(/submit/i));

  await findByText("Can't find email, please try again");
});

test('Enter valid email and submit, fetch returns back student, display SignInForm to student', async () => {
  const fakeFetch = jest.fn(url => {
    switch (url) {
      case `${backendURL}signins/test@wvup.edu/email`:
        return Promise.resolve({
          status: 200,
          json: () => Promise.resolve({ firstName: 'Test', lastName: 'User' })
        });
      case `${backendURL}reasons/active`:
        return Promise.resolve({
          status: 200,
          headers: [],
          json: () => Promise.resolve([])
        });
      default:
        return Promise.reject(Error("Didn't hit switch case statement"));
    }
  });
  global.fetch = fakeFetch;

  const { getByText, getByLabelText, findByText } = render(<SignInStudent />);

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: 'test@wvup.edu' }
  });
  fireEvent.blur(getByLabelText(/email/i));
  fireEvent.submit(getByText(/submit/i));

  await findByText(/Welcome, Test/i);
});
