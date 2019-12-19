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

const semesters = [
  { name: 'Fall 2019', code: 201901 },
  { name: 'Spring', code: 201902 },
  { name: 'Summer 2019', code: 201903 }
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
  personId: '2',
  firstName: 'Billy',
  lastName: 'Bob'
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
  personId: '4',
  firstName: 'Silly',
  lastName: 'Lob'
};

const existingTeacherRecord = {
  ...teacherSignIn,
  id: '401',
  selectedClasses: [],
  selectedReasons: [],
  inTime: '2019-05-10T14:00',
  outTime: '2019-05-10T16:00',
  semesterCode: semesters[1].code
};

const existingStudentSignIn = {
  ...studentSignIn,
  id: '5',
  inTime: '2019-01-05T09:00',
  outTime: '2019-01-05T12:00',
  selectedReasons: ['1'],
  selectedClasses: ['01234'],
  semesterCode: semesters[2].code
};

test('Renders with required props', () => {
  const { container } = render(
    <SignInForm
      signInRecord={studentSignIn}
      reasons={reasons}
      semesters={semesters}
    />
  );
  expect(container).toBeDefined();
});

test('Should render with submit button disabled', () => {
  const { getByText } = render(
    <SignInForm
      signInRecord={studentSignIn}
      reasons={reasons}
      semesters={semesters}
    />
  );
  expect(getByText(/submit/i)).toBeDisabled();
});

describe('Form Errors for Student', () => {
  test('Should display email, reasons and persons classSchedule when a student', () => {
    const { getByText } = render(
      <SignInForm
        signInRecord={studentSignIn}
        reasons={reasons}
        semesters={semesters}
      />
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
      <SignInForm
        signInRecord={studentSignIn}
        reasons={reasons}
        semesters={semesters}
      />
    );

    fireEvent.click(getByText(reasons[0].name));

    await wait(() => {
      expect(getByText(/submit/i)).toBeDisabled();
    });
  });

  test('Should have submit disabled if course is selected with no reason', async () => {
    const { getByText } = render(
      <SignInForm
        signInRecord={studentSignIn}
        reasons={reasons}
        semesters={semesters}
      />
    );

    fireEvent.click(getByText(studentSignIn.schedule[0].shortName));

    await wait(() => {
      expect(getByText(/submit/i)).toBeDisabled();
    });
  });
});

test('Should display correct time and date from signInRecord InTime and outTime', () => {
  const signIn = {
    email: '',
    selectedClasses: [],
    selectedReasons: [],
    inTime: '2019-05-13T13:40:39',
    outTime: '2019-05-13T16:31:31',
    personType: 0,
    tutoring: false,
    personId: '1',
    firstName: 'Sam',
    lastName: 'Puckett'
  };

  const { getByLabelText, debug } = render(
    <SignInForm signInRecord={signIn} reasons={reasons} semesters={semesters} />
  );

  debug();
  expect(getByLabelText(/in date/i)).toHaveValue('2019-05-13');
  expect(getByLabelText(/out date/i)).toHaveValue('2019-05-13');
  expect(getByLabelText(/in time/i)).toHaveValue('13:40:39');

  expect(getByLabelText(/out time/i)).toHaveValue('16:31:31');
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

    const { getByLabelText, getByText, getByTestId } = render(
      <SignInForm
        signInRecord={studentSignIn}
        reasons={reasons}
        semesters={semesters}
      />
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
    fireEvent.change(getByTestId('semester-select'), {
      target: { value: semesters[0].code }
    });

    fireEvent.click(getByText(reasons[0].name));
    fireEvent.click(getByText(studentSignIn.schedule[0].shortName));
    fireEvent.submit(getByText(/submit/i));

    expect(getByText(/submitting/i)).toBeDefined();

    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(`${backendURL}sessions/`, {
        headers: {
          'Content-Type': 'application/json'
        },
        method: 'POST',
        body: JSON.stringify({
          id: studentSignIn.id,
          personId: studentSignIn.personId,
          semesterCode: `${semesters[0].code}`,
          inTime: '2019-01-05T09:00',
          outTime: '2019-01-05T12:00',
          selectedClasses: ['01234'],
          selectedReasons: ['1'],
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

    const { getByLabelText, getByText, getByTestId } = render(
      <SignInForm signInRecord={teacherSignIn} semesters={semesters} />
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
    fireEvent.change(getByTestId('semester-select'), {
      target: { value: semesters[0].code }
    });

    fireEvent.submit(getByText(/submit/i));
    expect(getByText(/submitting/i)).toBeDefined();

    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(`${backendURL}sessions/`, {
        headers: {
          'Content-Type': 'application/json'
        },
        method: 'POST',
        body: JSON.stringify({
          id: teacherSignIn.id,
          personId: teacherSignIn.personId,
          semesterCode: `${semesters[0].code}`,
          inTime: `${inDT.date}T${inDT.time}`,
          outTime: `${outDT.date}T${outDT.time}`,
          selectedClasses: [],
          selectedReasons: [],
          tutoring: false
        })
      });
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
      <SignInForm
        signInRecord={existingStudentSignIn}
        reasons={reasons}
        semesters={semesters}
      />
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
        `${backendURL}sessions/${existingStudentSignIn.id}`,
        {
          headers: {
            'Content-Type': 'application/json'
          },
          method: 'PUT',
          body: JSON.stringify({
            id: existingStudentSignIn.id,
            personId: existingStudentSignIn.personId,
            semesterCode: existingStudentSignIn.semesterCode,
            inTime: '2019-01-05T09:00',
            outTime: '2019-01-05T14:00',
            selectedClasses: ['01234', '4567'],
            selectedReasons: [],
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
      <SignInForm signInRecord={existingTeacherRecord} semesters={semesters} />
    );

    fireEvent.change(getByLabelText(/in time/i), {
      target: { value: '12:00' }
    });

    fireEvent.submit(getByText(/submit/i));
    expect(getByText(/submitting/i)).toBeDefined();

    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(
        `${backendURL}sessions/${existingTeacherRecord.id}`,
        {
          headers: {
            'Content-Type': 'application/json'
          },
          method: 'PUT',
          body: JSON.stringify({
            id: existingTeacherRecord.id,
            personId: existingTeacherRecord.personId,
            semesterCode: existingTeacherRecord.semesterCode,
            inTime: '2019-05-10T12:00',
            outTime: '2019-05-10T16:00',
            selectedClasses: [],
            selectedReasons: [],
            tutoring: false
          })
        }
      );
    });
  });
});

test('Should display fetch response message when fetch returns non 2XX status code', async () => {
  const message = 'Something went terribly wrong while saving to database';
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      status: 400,
      json: () => Promise.resolve({ message })
    })
  );
  global.fetch = fakeFetch;

  const { getByLabelText, getByText, getByTestId } = render(
    <SignInForm signInRecord={studentSignIn} semesters={semesters} />
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
  fireEvent.change(getByTestId('semester-select'), {
    target: { value: semesters[0].code }
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
      <SignInForm signInRecord={person} semesters={semesters} />
    );

    fireEvent.change(getByLabelText(/in time/i), {
      target: { value: '24:00' }
    });
    await wait(() => {
      expect(getByText(/submit/i)).toBeDisabled();
    });
  });
});
