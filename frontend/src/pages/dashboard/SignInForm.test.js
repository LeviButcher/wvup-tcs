/* eslint-disable */
import React from 'react';
import '@testing-library/jest-dom/extend-expect';
import {
  render,
  fireEvent,
  cleanup,
  waitForElement,
  act,
  wait
} from 'CustomReactTestingLibrary'; // eslint-disable-line
import SignInForm from './SignInForm';

// put this in jest config
afterEach(cleanup);

const studentInfo = {
  studentEmail: 'lbutche3@wvup.edu',
  semesterId: 201903,
  studentID: 1,
  classSchedule: [
    {
      courseName: 'ART APPRECIATION',
      crn: 1,
      departmentID: 6,
      shortName: 'Art 101'
    }
  ]
};

const fetchWrapper = data =>
  Promise.resolve({
    json: () => Promise.resolve(data)
  });

const reasons = [
  {
    name: 'Computer Use',
    id: 5
  }
];

const mockFetchPromise = api => {
  // regex .test will return true matching the switch condition
  switch (true) {
    case /reasons\/active$/.test(api):
      return fetchWrapper(reasons);
    case /email$/.test(api):
      return fetchWrapper(studentInfo);
    default:
      throw Error('Wrong api');
  }
};
jest.spyOn(global, 'fetch').mockImplementation(mockFetchPromise);
jest.spyOn(global, 'alert').mockImplementation(value => value);

test('Invalid Email displays error', async () => {
  const {
    getByLabelText,
    getByText,
    findByLabelText,
    getById,
    findByText
  } = render(<SignInForm />);

  const emailInput = getByLabelText(/email/i);
  const email = 'MyFakeEmail@yahoo.com';

  act(() => {
    fireEvent.change(emailInput, { target: { value: email } });
  });

  act(() => {
    fireEvent.click(getByText(/submit/i));
  });

  expect(emailInput.value).toEqual(email);
  const emailError = await waitForElement(() => findByText(/address/i));
  expect(emailError).toBeDefined();
});

test('Reasons for visiting is loaded', async () => {
  const {
    getByLabelText,
    getByText,
    findByLabelText,
    getAllByLabelText,
    getById,
    findByText
  } = render(<SignInForm />);

  const [tutoring, computerUse] = await waitForElement(() => [
    findByLabelText(/tutoring/i),
    getAllByLabelText(/Computer Use/i)
  ]);

  expect(tutoring).toBeDefined();
  expect(computerUse).toBeDefined();
}, 9999);

// I wasted 4 hours on trying to get this to work, I will come back to this and defeat it
test('Ensure Tutoring has to be checked to submit', async () => {
  const {
    getByLabelText,
    getByText,
    findByLabelText,
    getById,
    findByText
  } = render(<SignInForm />);

  const emailInput = getByLabelText(/email/i);
  const email = 'lbutche3@wvup.edu';
  const tutoring = await waitForElement(() => findByLabelText(/tutoring/i));

  act(() => {
    fireEvent.change(emailInput, { target: { value: email } });
  });
  // fireEvent.click(tutoring);
  const submitButton = getByText(/submit/i);
  await wait(() => {
    expect(submitButton).toBeDisabled();
  });
});
