import React from 'react';
import { navigate, Router } from '@reach/router';
import {
  render,
  fireEvent,
  cleanup,
  wait,
  waitForElement
} from '../../test-utils/CustomReactTestingLibrary';
import ClassTourLookup from './ClassTourLookup';

test('Renders with required props', () => {
  const { container } = render(<ClassTourLookup navigate={jest.fn()} />);
  expect(container).toBeDefined();
});

describe('Class Tour Lookup', () => {
  test('Should display tours after submitting when searching with valid start and end date', async () => {
    const fakeFetch = jest.fn(() =>
      Promise.resolve({
        status: 200,
        headers: [],
        json: () =>
          Promise.resolve([
            { shortName: 'Ripley', name: 'Ripley', id: 1, crn: '12236' }
          ])
      })
    );
    global.fetch = fakeFetch;

    const { getByLabelText, getByText } = render(
      <Router>
        <ClassTourLookup navigate={navigate} default />
        <ClassTourLookup
          navigate={navigate}
          path="/dashboard/tours/:startDate/:endDate/"
        />
      </Router>
    );

    fireEvent.change(getByLabelText(/start/i), {
      target: { value: '2019-01-01' }
    });
    fireEvent.change(getByLabelText(/end/i), {
      target: { value: '2019-01-30' }
    });
    fireEvent.submit(getByText(/submit/i));

    const tour = await waitForElement(() => getByText(/Ripley/));
    expect(tour).toBeDefined();
  });
});
