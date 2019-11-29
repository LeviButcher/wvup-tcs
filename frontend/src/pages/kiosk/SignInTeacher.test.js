import React from 'react';
import {
  render,
  fireEvent,
  wait
} from '../../test-utils/CustomReactTestingLibrary';
import SignInTeacher from './SignInTeacher';
import toKeyCode from '../../test-utils/toKeyCode';

const backendURL = process.env.REACT_APP_BACKEND || '';

type Props = {
  email: string,
  id: number
};

// Used to mock out successful fetch calls during signIn process for teacher
const buildSignInProcessFetch = ({ email, id }: Props) => url => {
  switch (url) {
    case `${backendURL}signins/${id}/teacher/id`:
    case `${backendURL}signins/${email}/teacher/email`:
      return Promise.resolve({
        status: 200,
        json: () =>
          Promise.resolve({
            teacherID: id,
            teacherEmail: email
          })
      });
    case `${backendURL}signins?teacher=true`:
      return Promise.resolve({
        status: 201,
        json: () => Promise.resolve({})
      });
    default:
      return Promise.resolve({});
  }
};

test('Renders with required props', () => {
  const { container } = render(<SignInTeacher />);
  expect(container).toBeDefined();
});

test('Submit button disabled on load', () => {
  const { getByText } = render(<SignInTeacher />);

  const submit = getByText(/submit/i);
  expect(submit).toBeDisabled();
});

test('Non wvup email leaves submit button disabled with error message, change to valid enables submit and removes error', async () => {
  const { getByText, getByLabelText, queryByText } = render(<SignInTeacher />);

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: 'test@gmail.com' }
  });
  fireEvent.blur(getByLabelText(/email/i));

  await wait(() => {
    const submit = getByText(/submit/i);
    expect(submit).toBeDisabled();
    expect(getByText(/Must be a wvup/i)).toBeDefined();
  });

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: 'test@wvup.edu' }
  });
  fireEvent.blur(getByLabelText(/email/i));

  await wait(() => {
    const submit = getByText(/submit/i);
    expect(submit).not.toBeDisabled();
    expect(queryByText(/Must be a wvup/i)).toBeNull();
  });
});

test('Submit with valid wvup.edu email, calls fetch with correct backend endpoint', async () => {
  const fakeFetch = jest.fn(
    buildSignInProcessFetch({ email: 'test@wvup.edu', id: -1 })
  );
  global.fetch = fakeFetch;
  const { getByText, getByLabelText } = render(<SignInTeacher />);

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: 'test@wvup.edu' }
  });
  fireEvent.blur(getByLabelText(/email/i));

  fireEvent.submit(getByText(/submit/i));

  await wait(() => {
    expect(fakeFetch).toHaveBeenCalledTimes(2);
    expect(fakeFetch).toHaveBeenCalledWith(
      `${backendURL}signins/test@wvup.edu/teacher/email`,
      {
        headers: {
          'Content-Type': 'application/json'
        },
        method: 'GET'
      }
    );
  });
  // Shouldn't this pass in body for the teacher, email maybe?
  expect(fakeFetch).toHaveBeenCalledWith(`${backendURL}signins?teacher=true`, {
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ personId: -1, email: 'test@wvup.edu' }),
    method: 'POST'
  });
});

test('Card swipe calls fetch with correct backend endpoint', async () => {
  const fakeFetch = jest.fn(buildSignInProcessFetch({ id: 98, email: '' }));
  global.fetch = fakeFetch;

  const { container } = render(<SignInTeacher />);

  const card = ['%', 'l', 'b', '?', ';', '9', '8', '?', '\n'];

  card.forEach(char => fireEvent.keyPress(container, toKeyCode(char)));

  await wait(() => {
    expect(fakeFetch).toHaveBeenCalledTimes(2);
    expect(fakeFetch).toHaveBeenCalledWith(
      `${backendURL}signins/98/teacher/id`,
      {
        headers: {
          'Content-Type': 'application/json'
        },
        method: 'GET'
      }
    );
  });
});

test('During submission display is submitting text', async () => {
  const fakeFetch = jest.fn(
    buildSignInProcessFetch({ email: 'test@wvup.edu', id: -1 })
  );
  global.fetch = fakeFetch;

  const { getByText, getByLabelText } = render(<SignInTeacher />);

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: 'test@wvup.edu' }
  });
  fireEvent.blur(getByLabelText(/email/i));

  fireEvent.submit(getByText(/submit/i));
  expect(getByText(/submitting/i)).toBeDefined();

  await wait(() => {
    expect(fakeFetch).toHaveBeenCalledTimes(2);
  });
});

test('Display errors from bad fetch request', async () => {
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      status: 400,
      json: () => Promise.resolve({ message: 'Failed to call backend' })
    })
  );
  global.fetch = fakeFetch;

  const { getByText, getByLabelText } = render(<SignInTeacher />);

  fireEvent.change(getByLabelText(/email/i), {
    target: { value: 'test@wvup.edu' }
  });
  fireEvent.blur(getByLabelText(/email/i));

  fireEvent.submit(getByText(/submit/i));

  await wait(() => {
    expect(getByText(/Failed to call backend/i)).toBeDefined();
  });
});
