import React from 'react';
import {
  render,
  fireEvent,
  cleanup,
  wait,
  waitForElement
} from '../test-utils/CustomReactTestingLibrary';
import UserForm from './UserForm';

const backendURL = process.env.REACT_APP_BACKEND || '';

test('Renders with required props', () => {
  const { container } = render(<UserForm />);
  expect(container).toBeDefined();
});

test('Should render with submit disabled', () => {
  const { getByText } = render(<UserForm />);
  expect(getByText(/submit/i)).toBeDisabled();
});

describe('Form Errors', () => {
  test('Should not be able to submit with empty username', async () => {
    const { getByText, getByLabelText } = render(<UserForm />);

    fireEvent.change(getByLabelText(/password/i), {
      target: { value: 'Develop@90' }
    });
    fireEvent.change(getByLabelText(/first/i), {
      target: { value: 'Test' }
    });
    fireEvent.change(getByLabelText(/last/i), {
      target: { value: 'User' }
    });
    fireEvent.change(getByLabelText(/username/i), {
      target: { value: '' }
    });
    fireEvent.blur(getByLabelText(/username/i));
    const error = await waitForElement(() =>
      getByText(/username is required/i)
    );
    expect(error).toBeDefined();
    expect(getByText(/submit/i)).toBeDisabled();
  });

  test('Should not be able to submit with empty password when no data prop is passed in', async () => {
    const { getByText, getByLabelText } = render(<UserForm />);

    fireEvent.change(getByLabelText(/username/i), {
      target: { value: 'TCSEmployee1' }
    });
    fireEvent.change(getByLabelText(/first/i), {
      target: { value: 'Test' }
    });
    fireEvent.change(getByLabelText(/last/i), {
      target: { value: 'User' }
    });
    fireEvent.change(getByLabelText(/password/i), {
      target: { value: '' }
    });
    fireEvent.blur(getByLabelText(/password/i));

    const error = await waitForElement(() =>
      getByText(/password is required/i)
    );
    expect(error).toBeDefined();
    expect(getByText(/submit/i)).toBeDisabled();
  });

  test('Should be able to submit with empty password when user prop is passed in ', async () => {
    const { getByText, getByLabelText } = render(
      <UserForm
        user={{ firstName: 'Test', lastName: 'User', username: 'TCS' }}
      />
    );

    fireEvent.change(getByLabelText(/username/i), {
      target: { value: 'AdminTCS' }
    });
    fireEvent.change(getByLabelText(/password/i), {
      target: { value: '' }
    });
    fireEvent.blur(getByLabelText(/password/i));

    expect(getByText(/submit/i)).not.toBeDisabled();
  });

  test('Should be able to submit without filling in first name and last name', () => {
    const { getByText, getByLabelText } = render(<UserForm />);

    fireEvent.change(getByLabelText(/username/i), {
      target: { value: 'TCSEmployee1' }
    });
    fireEvent.change(getByLabelText(/first/i), {
      target: { value: 'Test' }
    });
    fireEvent.change(getByLabelText(/last/i), {
      target: { value: 'User' }
    });
    fireEvent.change(getByLabelText(/password/i), {
      target: { value: 'Develop@90' }
    });

    expect(getByText(/submit/i)).not.toBeDisabled();
  });

  test('Should display fetch error message after submit when fetch returns a non 2XX status code', async () => {
    const message = 'UserName already exists';
    const fakeFetch = jest.fn(() =>
      Promise.resolve({
        status: 400,
        headers: [],
        json: () => Promise.resolve({ message })
      })
    );
    global.fetch = fakeFetch;

    const { getByText, getByLabelText } = render(<UserForm />);

    fireEvent.change(getByLabelText(/username/i), {
      target: { value: 'TCSEmployee1' }
    });
    fireEvent.change(getByLabelText(/first/i), {
      target: { value: 'Test' }
    });
    fireEvent.change(getByLabelText(/last/i), {
      target: { value: 'User' }
    });
    fireEvent.change(getByLabelText(/password/i), {
      target: { value: 'Develop@90' }
    });

    fireEvent.submit(getByText(/submit/i));

    const errorMessage = await waitForElement(() => getByText(message));
    expect(errorMessage).toBeDefined();
  });
});

describe('Create User', () => {
  test('Should call fetch with correct arguments without passed in user prop', async () => {
    const fakeFetch = jest.fn(() =>
      Promise.resolve({
        status: 201,
        headers: [],
        json: () => Promise.resolve()
      })
    );
    global.fetch = fakeFetch;

    const { getByText, getByLabelText } = render(<UserForm />);

    fireEvent.change(getByLabelText(/username/i), {
      target: { value: 'TCSEmployee1' }
    });
    fireEvent.change(getByLabelText(/first/i), {
      target: { value: 'Test' }
    });
    fireEvent.change(getByLabelText(/last/i), {
      target: { value: 'User' }
    });
    fireEvent.change(getByLabelText(/password/i), {
      target: { value: 'Develop@90' }
    });
    fireEvent.submit(getByText(/submit/i));
    expect(getByText(/submitting/i)).toBeDefined();

    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(`${backendURL}users/register`, {
        headers: {
          'Content-Type': 'application/json'
        },
        method: 'POST',
        body: JSON.stringify({
          username: 'TCSEmployee1',
          firstName: 'Test',
          lastName: 'User',
          password: 'Develop@90'
        })
      });
    });
  });
});

describe('Update User', () => {
  test('Should call fetch with correct arguments when passed in user prop', async () => {
    const fakeFetch = jest.fn(() =>
      Promise.resolve({
        status: 200,
        headers: [],
        json: () => Promise.resolve()
      })
    );
    global.fetch = fakeFetch;

    const userToUpdate = {
      username: 'TCSEmployee2',
      lastName: 'Bob',
      firstName: 'Miller',
      id: '42'
    };

    const { getByText, getByLabelText } = render(
      <UserForm user={userToUpdate} />
    );
    expect(getByText(/update user/i)).toBeDefined();

    expect(getByLabelText(/username/i)).toHaveValue(userToUpdate.username);
    expect(getByLabelText(/last/i)).toHaveValue(userToUpdate.lastName);
    expect(getByLabelText(/first/i)).toHaveValue(userToUpdate.firstName);

    fireEvent.change(getByLabelText(/first/i), {
      target: { value: 'Builder' }
    });

    expect(getByText(/submit/i)).not.toBeDisabled();
    fireEvent.submit(getByText(/submit/i));
    expect(getByText(/submitting/i)).toBeDefined();

    await wait(() => {
      expect(fakeFetch).toHaveBeenCalledTimes(1);
      expect(fakeFetch).toHaveBeenCalledWith(
        `${backendURL}users/${userToUpdate.id}`,
        {
          headers: {
            'Content-Type': 'application/json'
          },
          method: 'PUT',
          body: JSON.stringify({
            username: userToUpdate.username,
            firstName: 'Builder',
            lastName: userToUpdate.lastName,
            password: '',
            id: userToUpdate.id
          })
        }
      );
    });
  });
});
