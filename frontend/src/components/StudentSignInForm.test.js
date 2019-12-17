/* eslint-disable import/named */
import React from 'react';
import {
  render,
  fireEvent,
  wait,
  waitForElement
} from '../test-utils/CustomReactTestingLibrary';
import StudentSignInForm from './StudentSignInForm';

const backendURL = process.env.REACT_APP_BACKEND || '';

const student = {
  firstName: 'Fake',
  lastName: 'Person',
  schedule: [{ crn: '31546', shortName: 'Math121' }],
  id: '1',
  email: 'fake@wvup.edu'
};

const reasons = [{ name: 'Computer Use', id: '1' }];

test('Renders with required props', () => {
  const { container } = render(<StudentSignInForm student={student} />);
  expect(container).toBeDefined();
});

test('Call backend api for reasons, display them as checkboxes and include tutoring checkbox', async () => {
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      status: 200,
      json: () => Promise.resolve(reasons),
      headers: []
    })
  );

  global.fetch = fakeFetch;

  const { findByLabelText } = render(<StudentSignInForm student={student} />);

  expect(await findByLabelText(/tutoring/i)).not.toBeChecked();

  // eslint-disable-next-line no-restricted-syntax
  for (const reason of reasons) {
    // eslint-disable-next-line no-await-in-loop
    expect(await findByLabelText(reason.name)).not.toBeChecked();
  }
});

test('Happy Path: Student Sign ins with tutoring, another reason, and 1 class, submit calls backend with correct fetch call', async () => {
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      status: 200,
      json: () => Promise.resolve(reasons),
      headers: []
    })
  );

  global.fetch = fakeFetch;

  const { getByLabelText, findByLabelText, getByText } = render(
    <StudentSignInForm student={student} />
  );

  fireEvent.click(await findByLabelText(/tutoring/i));
  fireEvent.click(getByLabelText(reasons[0].name));
  fireEvent.click(getByLabelText(student.schedule[0].shortName));
  fireEvent.submit(getByText(/submit/i));

  await wait(() => {
    expect(fakeFetch).toHaveBeenCalledTimes(2);
    expect(fakeFetch).toHaveBeenCalledWith(`${backendURL}session/signin`, {
      body: JSON.stringify({
        personId: '1',
        tutoring: true,
        selectedReasons: ['1'],
        selectedCourses: ['31546']
      }),
      headers: {
        'Content-Type': 'application/json'
      },
      method: 'POST'
    });
  });
});

test("During Submit, submit button disabled, after submit it's re-enabled", async () => {
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      status: 200,
      json: () => Promise.resolve(reasons),
      headers: []
    })
  );
  global.fetch = fakeFetch;

  const { getByLabelText, findByLabelText, getByText } = render(
    <StudentSignInForm student={student} />
  );

  fireEvent.click(await findByLabelText(/tutoring/i));
  fireEvent.click(getByLabelText(student.schedule[0].shortName));
  fireEvent.submit(getByText(/submit/i));
  expect(getByText(/submit/i)).toBeDisabled();

  await wait(() => {
    expect(getByText(/submit/i)).not.toBeDisabled();
  });
});

test('Should display fetch error message when fetch returns a non 2XX status code on submit', async () => {
  const error = { message: 'You are already signed in!' };
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      status: 200,
      json: () => Promise.resolve(reasons),
      headers: []
    })
  );

  global.fetch = fakeFetch;

  const { getByLabelText, findByLabelText, getByText } = render(
    <StudentSignInForm student={student} />
  );

  fireEvent.click(await findByLabelText(/tutoring/i));
  fireEvent.click(getByLabelText(reasons[0].name));
  fireEvent.click(getByLabelText(student.schedule[0].shortName));
  global.fetch = jest.fn(() =>
    Promise.resolve({ status: 400, json: () => Promise.resolve(error) })
  );
  fireEvent.submit(getByText(/submit/i));

  const errorMessage = await waitForElement(() => getByText(error.message));
  expect(errorMessage).toBeDefined();
});
