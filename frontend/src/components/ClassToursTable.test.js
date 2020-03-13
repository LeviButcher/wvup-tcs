import React from 'react';
import {
  render,
  fireEvent,
  wait
} from '../test-utils/CustomReactTestingLibrary';
import ClassToursTable from './ClassToursTable';

const backendURL = process.env.REACT_APP_BACKEND || '';
const classToursData = [
  {
    id: '1',
    name: 'South High',
    numberOfStudents: 25,
    dayVisited: ''
  }
];

test('Should render successfully', () => {
  const { container } = render(<ClassToursTable classTours={classToursData} />);
  expect(container).toBeDefined();
});

test('Should make correct api call and refresh page when delete is clicked', async () => {
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      status: 200,
      json: () => Promise.resolve()
    })
  );
  const reloadWindow = jest.fn();
  global.fetch = fakeFetch;
  window.location.reload = reloadWindow;
  window.confirm = () => true;

  const { getByTestId } = render(
    <ClassToursTable classTours={classToursData} />
  );
  fireEvent.click(getByTestId(/delete-classtour/i));
  expect(fakeFetch).toHaveBeenCalledWith(
    `${backendURL}classtours/${classToursData[0].id}`,
    {
      body: 'null',
      headers: {
        'Content-Type': 'application/json'
      },
      method: 'DELETE'
    }
  );
  await wait(() => {
    expect(reloadWindow).toHaveBeenCalled();
  });
});
