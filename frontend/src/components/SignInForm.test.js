/* eslint-disable import/named */
import React from 'react';
import {
  render,
  fireEvent,
  wait,
  waitForElement
} from '../test-utils/CustomReactTestingLibrary';
import SignInForm from './SignInForm';
import { personTypeValues } from '../types';

const backendURL = process.env.REACT_APP_BACKEND || '';

const reasons = [
  { id: '1', name: 'Computer Use', deleted: false },
  { id: '2', name: 'Bone Use', deleted: true }
];

const studentSignIn = {
  email: 'something@wvup.edu',
  selectedReasons: [],
  schedule: [
    { crn: '01234', shortName: 'CS101' },
    { crn: '4567', shortName: 'STEM329' }
  ],
  selectedClasses: [],
  inTime: '',
  outTime: '',
  tutoring: false,
  id: '',
  personType: personTypeValues.student,
  semesterId: '201902',
  personId: '2'
};

const teacherSignIn = {
  email: 'teacher@wvup.edu',
  inTime: '',
  outTime: '',
  selectedClasses: [],
  selectedReasons: [],
  tutoring: false,
  id: '',
  personType: personTypeValues.teacher,
  semesterId: '201902',
  personId: '4'
};

const existingTeacherRecord = {
  ...teacherSignIn,
  id: '401',
  selectedClasses: [],
  selectedReasons: [],
  inTime: '2019-05-10 14:00',
  outTime: '2019-05-10 16:00'
};

const existingStudentSignIn = {
  ...studentSignIn,
  id: '5',
  inTime: '2019-01-05 09:00',
  outTime: '2019-01-05 12:00',
  selectedReasons: ['1'],
  selectedClasses: ['01234']
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

describe('Form Errors for Student', () => {
  test('Should display email, reasons and persons classSchedule when a student', () => {
    const { getByText } = render(
      <SignInForm signInRecord={studentSignIn} reasons={reasons} />
    );
    reasons.forEach(reason => {
      expect(getByText(reason.name)).toBeDefined();
    });
    studentSignIn.schedule.forEach(course => {
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

    fireEvent.click(getByText(studentSignIn.schedule[0].shortName));

    await wait(() => {
      expect(getByText(/submit/i)).toBeDisabled();
    });
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

    const { getByLabelText, getByText } = render(
      <SignInForm signInRecord={studentSignIn} reasons={reasons} />
    );

    fireEvent.change(getByLabelText(/in date/i), {
      target: { value: '2019-01-05' }
    });
    fireEvent.change(getByLabelText(/out date/i), {
      target: { value: '2019-01-05' }
    });
    fireEvent.change(getByLabelText(/in time/i), {
      target: { value: '09:00' }
    });
    fireEvent.change(getByLabelText(/out time/i), {
      target: { value: '12:00' }
    });

    fireEvent.click(getByText(reasons[0].name));
    fireEvent.click(getByText(studentSignIn.schedule[0].shortName));
    fireEvent.submit(getByText(/submit/i));
    expect(getByText(/submitting/i)).toBeDefined();

    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(`${backendURL}signIns/admin/`, {
        headers: {
          'Content-Type': 'application/json'
        },
        method: 'POST',
        body: JSON.stringify({
          id: studentSignIn.id,
          personId: studentSignIn.personId,
          semesterId: studentSignIn.semesterId,
          inTime: new Date('2019-01-05 09:00'),
          outTime: new Date('2019-01-05 12:00'),
          courses: ['01234'],
          reasons: ['1'],
          tutoring: false
        })
      });
    });
  });

  test('Should call fetch with expected arguments when person is a teacher', async () => {
    const fakeFetch = jest.fn(() =>
      Promise.resolve({
        status: 201,
        json: () => Promise.resolve()
      })
    );
    global.fetch = fakeFetch;

    const { getByLabelText, getByText } = render(
      <SignInForm signInRecord={teacherSignIn} />
    );
    const inDT = { date: '2019-05-10', time: '14:00' };
    const outDT = { date: '2019-05-11', time: '16:00' };

    fireEvent.change(getByLabelText(/in date/i), {
      target: { value: inDT.date }
    });
    fireEvent.change(getByLabelText(/out date/i), {
      target: { value: outDT.date }
    });
    fireEvent.change(getByLabelText(/in time/i), {
      target: { value: inDT.time }
    });
    fireEvent.change(getByLabelText(/out time/i), {
      target: { value: outDT.time }
    });

    fireEvent.submit(getByText(/submit/i));
    expect(getByText(/submitting/i)).toBeDefined();

    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(
        `${backendURL}signIns/admin/?teacher=true`,
        {
          headers: {
            'Content-Type': 'application/json'
          },
          method: 'POST',
          body: JSON.stringify({
            id: teacherSignIn.id,
            personId: teacherSignIn.personId,
            semesterId: teacherSignIn.semesterId,
            inTime: new Date(`${inDT.date} ${inDT.time}`),
            outTime: new Date(`${outDT.date} ${outDT.time}`),
            courses: [],
            reasons: [],
            tutoring: false
          })
        }
      );
    });
  });
});

describe('Update SignIn', () => {
  test('Should call fetch with expected arguments when person is a student and updating an existing signInRecord', async () => {
    const fakeFetch = jest.fn(() =>
      Promise.resolve({
        status: 200,
        json: () => Promise.resolve()
      })
    );
    global.fetch = fakeFetch;

    const { getByLabelText, getByText } = render(
      <SignInForm signInRecord={existingStudentSignIn} reasons={reasons} />
    );

    fireEvent.change(getByLabelText(/out time/i), {
      target: { value: '14:00' }
    });

    fireEvent.click(getByText(reasons[0].name));
    fireEvent.click(getByText(studentSignIn.schedule[1].shortName));
    fireEvent.click(getByLabelText(/tutoring/i));
    fireEvent.submit(getByText(/submit/i));
    expect(getByText(/submitting/i)).toBeDefined();
    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(
        `${backendURL}signIns/${existingStudentSignIn.id}`,
        {
          headers: {
            'Content-Type': 'application/json'
          },
          method: 'PUT',
          body: JSON.stringify({
            id: existingStudentSignIn.id,
            personId: existingStudentSignIn.personId,
            semesterId: existingStudentSignIn.semesterId,
            inTime: new Date('2019-01-05 09:00'),
            outTime: new Date('2019-01-05 14:00'),
            courses: ['01234', '4567'],
            reasons: [],
            tutoring: true
          })
        }
      );
    });
  });

  test('Should call fetch with expected arguments when person is a teacher and updating an existing signInRecord', async () => {
    const fakeFetch = jest.fn(() =>
      Promise.resolve({
        status: 200,
        json: () => Promise.resolve()
      })
    );
    global.fetch = fakeFetch;

    const { getByLabelText, getByText } = render(
      <SignInForm signInRecord={existingTeacherRecord} />
    );

    fireEvent.change(getByLabelText(/in time/i), {
      target: { value: '12:00' }
    });

    fireEvent.submit(getByText(/submit/i));
    expect(getByText(/submitting/i)).toBeDefined();

    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(
        `${backendURL}signIns/${existingTeacherRecord.id}?teacher=true`,
        {
          headers: {
            'Content-Type': 'application/json'
          },
          method: 'PUT',
          body: JSON.stringify({
            id: existingTeacherRecord.id,
            personId: existingTeacherRecord.personId,
            semesterId: existingTeacherRecord.semesterId,
            inTime: new Date('2019-05-10 12:00'),
            outTime: new Date('2019-05-10 16:00'),
            courses: [],
            reasons: [],
            tutoring: false
          })
        }
      );
    });
  });
});

// NEED MORE TESTS
// [ ] Check status is displayed from fetch error
// [ ] Student Form errors on date, intime after outTime
// [ ] Teacher Form Errors, inTime after outTime

test('Should display fetch response message when fetch returns non 2XX status code', async () => {
  const message = 'Something went terribly wrong while saving to database';
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      status: 400,
      json: () => Promise.resolve({ message })
    })
  );
  global.fetch = fakeFetch;

  const { getByLabelText, getByText } = render(
    <SignInForm signInRecord={studentSignIn} />
  );
  const inDT = { date: '2019-05-10', time: '14:00' };
  const outDT = { date: '2019-05-11', time: '16:00' };

  fireEvent.change(getByLabelText(/in date/i), {
    target: { value: inDT.date }
  });
  fireEvent.change(getByLabelText(/out date/i), {
    target: { value: outDT.date }
  });
  fireEvent.change(getByLabelText(/in time/i), {
    target: { value: inDT.time }
  });
  fireEvent.change(getByLabelText(/out time/i), {
    target: { value: outDT.time }
  });

  fireEvent.click(getByLabelText(/tutoring/i));
  fireEvent.click(getByText(studentSignIn.schedule[0].shortName));
  fireEvent.submit(getByText(/submit/i));

  const error = await waitForElement(() => getByText(message));
  expect(error).toBeDefined();
});

[existingTeacherRecord, existingStudentSignIn].forEach(person => {
  test(`Should not be able to submit if inTime is after outTime when person is ${person.personType}`, async () => {
    const { getByLabelText, getByText } = render(
      <SignInForm signInRecord={person} />
    );

    fireEvent.change(getByLabelText(/in time/i), {
      target: { value: '24:00' }
    });
    await wait(() => {
      expect(getByText(/submit/i)).toBeDisabled();
    });
  });
});
