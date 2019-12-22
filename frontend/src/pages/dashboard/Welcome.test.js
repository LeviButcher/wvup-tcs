import React from 'react';
import Welcome from './Welcome';
import {
  render,
  waitForElement
} from '../../test-utils/CustomReactTestingLibrary';

const backendURL = process.env.REACT_APP_BACKEND || '';

const signIns = [
  {
    id: 134,
    inTime: '2019-05-04T20:00',
    outTime: '2019-05-04T22:00',
    selectedClasses: [{ name: 'Intro to Computing' }],
    selectedReasons: [{ name: 'Computer Use' }],
    person: {
      email: 'lbutche3@wvup.edu',
      firstName: 'Levi',
      lastName: 'Butters'
    }
  }
];

test('Should render successfully', () => {
  const { container } = render(<Welcome />);
  expect(container).toBeDefined();
});

test("Should call fetch with correct args and display today's sessions on page", async () => {
  const today = new Date().toLocaleDateString();
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      status: 200,
      json: () =>
        Promise.resolve({
          totalPages: 5,
          currentPage: 1,
          totalRecords: 50,
          data: signIns
        })
    })
  );
  global.fetch = fakeFetch;

  const { getByText } = render(<Welcome />);

  const studentsHelped = await waitForElement(() => getByText(/50/));
  expect(studentsHelped).toBeDefined();
  const signInRecord = getByText(signIns[0].person.email.split('@')[0]);
  expect(signInRecord).toBeDefined();

  expect(fakeFetch).toHaveBeenCalledTimes(1);
  expect(fakeFetch).toHaveBeenCalledWith(
    `${backendURL}sessions?start=${today}&end=${today}`,
    expect.anything()
  );
});
