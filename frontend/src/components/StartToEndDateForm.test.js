import React from 'react';
import {
  render,
  fireEvent,
  cleanup,
  wait,
  waitForElement
} from '../test-utils/CustomReactTestingLibrary';
import StartToEndDateForm from './StartToEndDateForm';

test('Renders with required props', () => {
  const { container } = render(
    <StartToEndDateForm title="Test Title" onSubmit={jest.fn()} />
  );
  expect(container).toBeDefined();
});

test('Should have submit button disabled on render', () => {
  const { getByText } = render(
    <StartToEndDateForm title="Test Title" onSubmit={jest.fn()} />
  );
  expect(getByText(/submit/i)).toBeDisabled();
});

test('Should not have submit disabled if endDate is after startDate', async () => {
  const { getByLabelText, getByText } = render(
    <StartToEndDateForm title="Test Title" onSubmit={jest.fn()} />
  );

  fireEvent.change(getByLabelText(/start/i), {
    target: { value: '2019-01-09' }
  });
  fireEvent.change(getByLabelText(/end/i), {
    target: { value: '2019-01-30' }
  });

  await wait(() => {
    expect(getByText(/submit/i)).not.toBeDisabled();
  });
});

test('Should have submit disabled if EndDate is before StartDate', async () => {
  const { getByLabelText, getByText } = render(
    <StartToEndDateForm title="Test Title" onSubmit={jest.fn()} />
  );

  fireEvent.change(getByLabelText(/start/i), {
    target: { value: '2019-01-30' }
  });
  fireEvent.change(getByLabelText(/end/i), {
    target: { value: '2019-01-01' }
  });

  await wait(() => {
    expect(getByText(/submit/i)).toBeDisabled();
  });
});
test('Should call onSubmit with form values when submitted', async () => {
  const mockSubmit = jest.fn();
  const { getByLabelText, getByText } = render(
    <StartToEndDateForm title="Test Title" onSubmit={mockSubmit} />
  );

  fireEvent.change(getByLabelText(/start/i), {
    target: { value: '2019-01-01' }
  });
  fireEvent.change(getByLabelText(/end/i), {
    target: { value: '2019-01-30' }
  });
  fireEvent.submit(getByText(/submit/i));

  const submitting = await waitForElement(() => getByText(/submitting/i));
  expect(submitting).toBeDefined();

  await wait(() => {
    expect(mockSubmit).toHaveBeenCalledTimes(1);
    expect(mockSubmit).toHaveBeenCalledWith(
      {
        startDate: '2019-01-01',
        endDate: '2019-01-30'
      },
      expect.anything()
    );
  });
});
