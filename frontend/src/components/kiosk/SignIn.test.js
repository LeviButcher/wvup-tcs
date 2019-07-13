/* eslint-disable */
import React from 'react';
import {
  render,
  fireEvent,
  cleanup,
  waitForElement,
  act
} from 'CustomReactTestingLibrary'; // eslint-disable-line
import SignIn from './SignIn';

// put this in jest config
afterEach(cleanup);

const mockSuccess = {
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
const mockFetchPromise = Promise.resolve({
  json: () => Promise.resolve(mockSuccess)
});
jest.spyOn(global, 'fetch').mockImplementation(() => mockFetchPromise);
jest.spyOn(global, 'alert').mockImplementation(value => value);

test('Invalid Email displays error', async () => {
  const {
    getByLabelText,
    getByText,
    findByLabelText,
    getById,
    findByText
  } = render(<SignIn />);

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
  } = render(<SignIn />);

  const [tutoring, computerUse] = await waitForElement(() => [
    findByLabelText(/tutoring/i),
    getAllByLabelText(/Computer Use/i)
  ]);

  expect(tutoring).toBeDefined();
  expect(computerUse).toBeDefined();
}, 9999);

// I wasted 4 hours on trying to get this to work, I will come back to this and defeat it
// test('Ensure Tutoring has to be checked to submit', async () => {
//   const {
//     getByLabelText,
//     getByText,
//     findByLabelText,
//     getById,
//     findByText,
//     debug
//   } = render(<SignIn />);
//
//   const emailInput = getByLabelText(/email/i);
//   const email = 'lbutche3@wvup.edu';
//   const tutoring = await waitForElement(() => findByLabelText(/tutoring/i));
//
//   act(() => {
//     fireEvent.change(emailInput, { target: { value: email } });
//   });
//   // fireEvent.click(tutoring);
//   debug();
//   const submitButton = getByText(/submit/i);
//   expect(submitButton.disabled).toBe(true);
// });
