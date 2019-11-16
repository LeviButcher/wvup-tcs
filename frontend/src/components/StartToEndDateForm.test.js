import React from 'react';
import '@testing-library/jest-dom/extend-expect';
import {
  render,
  fireEvent,
  cleanup,
  wait
} from '../test-utils/CustomReactTestingLibrary';
import StartToEndDateForm from './StartToEndDateForm';

afterEach(cleanup);

test('EndDate has to be after StartDate to submit', async () => {
  const { getByLabelText, getByText } = render(<StartToEndDateForm />);
  const startDate = getByLabelText(/start/i);
  const endDate = getByLabelText(/end/i);
  const submit = getByText(/run report/i);
  const date = new Date();

  fireEvent.change(startDate, {
    target: { value: date.toISOString().substr(0, 10) }
  });
  date.setDate(date.getDate() - 5);
  fireEvent.change(endDate, {
    target: { value: date.toISOString().substr(0, 10) }
  });

  await wait(() => {
    expect(submit).toBeDisabled();
  });
});
