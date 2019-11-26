/* eslint-disable import/named */
import React from 'react';
import {
  render,
  fireEvent,
  wait
} from '../test-utils/CustomReactTestingLibrary';
import SignInForm from './SignInForm';

const backendURL = process.env.REACT_APP_BACKEND || '';

const reasons = [{ id: '1', name: 'Computer Use', deleted: false }];

const studentSignIn = {
  email: 'something@wvup.edu',
  selectedReasons: [],
  classSchedule: [{ crn: '01234', shortName: 'CS101' }],
  selectedClasses: [],
  inTime: '',
  outTime: '',
  tutoring: false,
  id: '55',
  personType: 'Student',
  semesterId: '201902',
  personId: '2'
};

test('Renders with required props', () => {
  const { container } = render(
    <SignInForm signInRecord={studentSignIn} reasons={reasons} />
  );
  expect(container).toBeDefined();
});

test('Should render with submit button disabled', () => {
  const { getByText } = render(
    <SignInForm signInRecord={studentSignIn} reasons={reasons} />
  );
  expect(getByText(/submit/i)).toBeDisabled();
});

test('Should display email, reasons and persons classSchedule when a student', () => {
  const { getByText } = render(
    <SignInForm signInRecord={studentSignIn} reasons={reasons} />
  );
  reasons.forEach(reason => {
    expect(getByText(reason.name)).toBeDefined();
  });
  studentSignIn.classSchedule.forEach(course => {
    expect(getByText(course.shortName)).toBeDefined();
  });
});

test('Should have submit disabled if reason has been selected with no courses', async () => {
  const { getByText } = render(
    <SignInForm signInRecord={studentSignIn} reasons={reasons} />
  );

  fireEvent.click(getByText(reasons[0].name));

  await wait(() => {
    expect(getByText(/submit/i)).toBeDisabled();
  });
});

test('Should have submit disabled if course is selected with no reason', async () => {
  const { getByText } = render(
    <SignInForm signInRecord={studentSignIn} reasons={reasons} />
  );

  fireEvent.click(getByText(studentSignIn.classSchedule[0].shortName));

  await wait(() => {
    expect(getByText(/submit/i)).toBeDisabled();
  });
});

describe('Create SignIn', () => {
  // Fails because inTime and outTime have to be set before onSubmit will ever be executed.
  test('Should call fetch with expected arguments when person is a student', async () => {
    const fakeFetch = jest.fn(() =>
      Promise.resolve({
        status: 201,
        json: () => Promise.resolve()
      })
    );
    global.fetch = fakeFetch;

    const { getByText } = render(
      <SignInForm signInRecord={studentSignIn} reasons={reasons} />
    );

    fireEvent.click(getByText(reasons[0].name));
    fireEvent.click(getByText(studentSignIn.classSchedule[0].shortName));
    fireEvent.submit(getByText(/submit/i));
    expect(getByText(/submitting/i)).toBeDefined();

    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(`${backendURL}signIns/admin/`);
    });
  });
});
