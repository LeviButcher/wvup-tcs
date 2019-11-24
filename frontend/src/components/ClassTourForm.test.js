import React from 'react';
import {
  render,
  fireEvent,
  wait
} from '../test-utils/CustomReactTestingLibrary';
import ClassTourForm from './ClassTourForm';

const backendURL = process.env.REACT_APP_BACKEND || '';

test('Should render with required props', () => {
  const { container } = render(<ClassTourForm />);
  expect(container).toBeDefined();
});

test('Should render with submit button disabled', () => {
  const { getByText } = render(<ClassTourForm />);
  expect(getByText(/submit/i)).toBeDisabled();
});

describe('Form Error Handling', () => {
  test('Should not be able to submit with negative number of students', async () => {
    const { getByText, getByLabelText } = render(<ClassTourForm />);

    fireEvent.change(getByLabelText(/name/i), {
      target: { value: 'Ripley' }
    });
    fireEvent.blur(getByLabelText(/name/i));
    fireEvent.change(getByLabelText(/date/i), {
      target: { value: '2019-03-29' }
    });
    fireEvent.blur(getByLabelText(/date/i));
    fireEvent.change(getByLabelText(/tourist/i), {
      target: { value: -1 }
    });
    fireEvent.blur(getByLabelText(/tourist/i));

    await wait(() => {
      expect(getByText(/submit/i)).toBeDisabled();
    });
  });

  test('Should not be able to submit with empty strings name', async () => {
    const { getByText, getByLabelText } = render(<ClassTourForm />);

    fireEvent.change(getByLabelText(/name/i), {
      target: {
        value: ' '
      }
    });
    fireEvent.blur(getByLabelText(/name/i));

    fireEvent.change(getByLabelText(/date/i), {
      target: {
        value: '2019-03-29'
      }
    });
    fireEvent.blur(getByLabelText(/date/i));

    fireEvent.change(getByLabelText(/tourist/i), {
      target: {
        value: 20
      }
    });
    fireEvent.blur(getByLabelText(/tourist/i));

    await wait(() => {
      expect(getByText(/submit/i)).toBeDisabled();
    });
  });

  test('Should be able to with all valid values', async () => {
    const { getByText, getByLabelText } = render(<ClassTourForm />);

    fireEvent.change(getByLabelText(/name/i), {
      target: { value: 'Ripley' }
    });
    fireEvent.blur(getByLabelText(/name/i));

    fireEvent.change(getByLabelText(/date/i), {
      target: { value: '2019-03-05' }
    });
    fireEvent.blur(getByLabelText(/date/i));

    fireEvent.change(getByLabelText(/tourist/i), {
      target: { value: 20 }
    });
    fireEvent.blur(getByLabelText(/tourist/i));

    await wait(() => {
      expect(getByText(/submit/i)).not.toBeDisabled();
    });
  });
});

describe('User submitting form', () => {
  test('Should call fetch with correct values when submitting valid values', async () => {
    const fakeFetch = jest.fn(() =>
      Promise.resolve({ status: 201, json: () => Promise.resolve() })
    );
    global.fetch = fakeFetch;
    const { getByText, getByLabelText, findByText } = render(<ClassTourForm />);

    fireEvent.change(getByLabelText(/name/i), {
      target: { value: 'Ripley' }
    });
    fireEvent.blur(getByLabelText(/name/i));

    fireEvent.change(getByLabelText(/date/i), {
      target: { value: '2019-03-05' }
    });
    fireEvent.blur(getByLabelText(/date/i));

    fireEvent.change(getByLabelText(/tourist/i), {
      target: { value: 20 }
    });
    fireEvent.blur(getByLabelText(/tourist/i));
    fireEvent.submit(getByText(/submit/i));
    expect(await findByText(/submitting/i)).toBeDefined();

    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(`${backendURL}classtours/`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          name: 'Ripley',
          dayVisited: '2019-03-05',
          numberOfStudents: 20
        })
      });
    });
  });

  test('Should display error message when fetch returns back non 2xx status code', async () => {
    const errorMessage = 'Submission failed, will get them next time';
    const fakeFetch = jest.fn(() =>
      Promise.resolve({
        status: 400,
        json: () =>
          Promise.resolve({
            message: errorMessage
          })
      })
    );
    global.fetch = fakeFetch;
    const { getByText, getByLabelText, findByText } = render(<ClassTourForm />);

    fireEvent.change(getByLabelText(/name/i), {
      target: { value: 'Ripley' }
    });
    fireEvent.blur(getByLabelText(/name/i));

    fireEvent.change(getByLabelText(/date/i), {
      target: { value: '2019-03-05' }
    });
    fireEvent.blur(getByLabelText(/date/i));

    fireEvent.change(getByLabelText(/tourist/i), {
      target: { value: 20 }
    });
    fireEvent.blur(getByLabelText(/tourist/i));
    fireEvent.submit(getByText(/submit/i));

    const error = await findByText(errorMessage);
    expect(error).toBeDefined();
  });
});

describe('Update Class Tour', () => {
  test('Should display passed in classTour values and Update Tour text', () => {
    const fakeTour = {
      dayVisited: '2019-01-30',
      name: 'RipCity',
      numberOfStudents: 1,
      id: '5'
    };
    const { getByText, getByLabelText } = render(
      <ClassTourForm classTour={fakeTour} />
    );
    expect(getByText(/Update Tour/i)).toBeDefined();
    expect(getByLabelText(/name/i)).toHaveValue(fakeTour.name);
    expect(getByLabelText(/date/i)).toHaveValue(fakeTour.dayVisited);
    expect(getByLabelText(/tourist/i)).toHaveValue(fakeTour.numberOfStudents);
  });

  test('Should call fetch with correct backend api with data on submission when passed in a classTour', async () => {
    const fakeFetch = jest.fn(() => Promise.resolve({ status: 200 }));
    global.fetch = fakeFetch;
    const fakeTour = {
      dayVisited: '2019-01-30',
      name: 'RipCity',
      numberOfStudents: 1,
      id: '5'
    };
    const { getByText, getByLabelText } = render(
      <ClassTourForm classTour={fakeTour} />
    );

    fireEvent.change(getByLabelText(/tourist/i), {
      target: { value: 20 }
    });
    fireEvent.blur(getByLabelText(/tourist/i));
    fireEvent.submit(getByText(/submit/i));

    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(
        `${backendURL}classtours/${fakeTour.id}`,
        {
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({ ...fakeTour, numberOfStudents: 20 }),
          method: 'PUT'
        }
      );
    });
  });
});
