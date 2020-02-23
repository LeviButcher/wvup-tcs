import React from 'react';
import { Router } from '@reach/router';
import {
  render,
  fireEvent,
  wait
} from '../../test-utils/CustomReactTestingLibrary';
import ReasonsReport from './ReasonForVisitingReport';

const backendURL = process.env.REACT_APP_BACKEND || '';

test('Submit start and end date, display data returned from fetch call', async () => {
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      status: 200,
      json: () =>
        Promise.resolve([
          {
            className: 'CS101',
            classCRN: '13456',
            visits: 49,
            reasonName: 'Computer Use'
          },
          {
            className: 'Math 156',
            classCRN: '45679',
            visits: 25,
            reasonName: 'Whiteboard Use'
          },
          {
            className: 'CS121',
            classCRN: '46871',
            visits: 26,
            reasonName: 'Computer Use'
          }
        ])
    })
  );
  global.fetch = fakeFetch;

  const { getByLabelText, getByText, findByText } = render(
    <Router>
      <ReasonsReport
        path="*"
        navigate={jest.fn()}
        {...{
          '*': ''
        }}
      />
    </Router>
  );

  fireEvent.change(getByLabelText(/start/i), {
    target: {
      value: '2020-01-01'
    }
  });
  fireEvent.change(getByLabelText(/end/i), {
    target: {
      value: '2020-02-01'
    }
  });

  fireEvent.submit(getByText(/submit/i));

  await findByText(/Reason For Visiting Summary/i);

  // Tests Table Group of Reason Name
  expect(getByText(/Computer Use -/i)).not.toBeNull();
  expect(getByText(/CS101/i)).not.toBeNull();
  expect(getByText(/Whiteboard Use -/i)).not.toBeNull();
  expect(getByText(/Math 156/i)).not.toBeNull();

  // Assert for Pie Chart
  expect(getByText(/Reason For Visiting Percentages/i)).not.toBeNull();
  expect(getByText(/75.0/i)).not.toBeNull();
  expect(getByText(/25.0/i)).not.toBeNull();

  expect(fakeFetch).toHaveBeenCalledWith(
    `${backendURL}reports/reasons?start=2020-01-01&end=2020-02-01`,
    expect.anything()
  );
});
