import React from 'react';
import {
  render,
  fireEvent,
  cleanup,
  wait,
  waitForElement
} from '../test-utils/CustomReactTestingLibrary';
import ReasonForm from './ReasonForm';

const backendURL = process.env.REACT_APP_BACKEND || '';

test('Renders with required props', () => {
  const { container } = render(<ReasonForm />);
  expect(container).toBeDefined();
});

test('Should render with submit button disabled', () => {
  const { getByText } = render(<ReasonForm />);
  expect(getByText(/submit/i)).toBeDisabled();
});

describe('Form Errors', () => {
  test('Should have submit disabled when name is empty string', async () => {
    const { getByLabelText, getByText } = render(<ReasonForm />);

    fireEvent.click(getByLabelText(/deleted/i));
    fireEvent.change(getByLabelText(/name/i), { target: { value: '' } });
    fireEvent.blur(getByLabelText(/name/i));
    const error = await waitForElement(() => getByText(/name is required/i));
    expect(error).toBeDefined();

    expect(getByText(/submit/i)).toBeDisabled();
  });

  test('Should display fetch error message when fetch returns non 2XX status code', async () => {
    const message = 'Book use already exists';
    const fakeFetch = jest.fn(() =>
      Promise.resolve({
        status: 400,
        json: () => Promise.resolve({ message })
      })
    );
    global.fetch = fakeFetch;

    const { getByLabelText, getByText } = render(<ReasonForm />);

    fireEvent.change(getByLabelText(/name/i), {
      target: { value: 'Book Use' }
    });
    fireEvent.submit(getByText(/submit/i));

    const error = await waitForElement(() => getByText(message));
    expect(error).toBeDefined();
  });
});

describe('Create Reason', () => {
  test('Should call fetch with correct arguments when passed in no reason prop', async () => {
    const fakeFetch = jest.fn(() =>
      Promise.resolve({
        status: 201,
        json: () => Promise.resolve({})
      })
    );
    global.fetch = fakeFetch;

    const { getByLabelText, getByText } = render(<ReasonForm />);

    expect(getByText(/Create Reason/)).toBeDefined();

    fireEvent.change(getByLabelText(/name/i), {
      target: { value: 'Computer Use' }
    });
    fireEvent.submit(getByText(/submit/i));
    expect(getByText(/submitting/i)).toBeDefined();

    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(`${backendURL}reasons/`, {
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          name: 'Computer Use',
          deleted: false
        }),
        method: 'POST'
      });
    });
  });
});

describe('Update Reason', () => {
  test('Should call fetch with correct arguments when passed in reason prop', async () => {
    const fakeFetch = jest.fn(() =>
      Promise.resolve({
        status: 200,
        json: () => Promise.resolve({})
      })
    );
    global.fetch = fakeFetch;
    const reasonToUpdate = {
      name: 'Testing Center',
      deleted: false,
      id: '49'
    };

    const { getByLabelText, getByText } = render(
      <ReasonForm reason={reasonToUpdate} />
    );

    expect(getByText(/Update Reason/)).toBeDefined();

    fireEvent.click(getByLabelText(/deleted/i));
    fireEvent.submit(getByText(/submit/i));
    expect(getByText(/submitting/i)).toBeDefined();

    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(
        `${backendURL}reasons/${reasonToUpdate.id}`,
        {
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({
            name: 'Testing Center',
            deleted: true,
            id: '49'
          }),
          method: 'PUT'
        }
      );
    });
  });
});

test('Should not have Deleted Checkbox visible when reason prop is not passed in', () => {
  const { getByLabelText } = render(<ReasonForm />);
  expect(getByLabelText(/deleted/i)).not.toBeVisible();
});
