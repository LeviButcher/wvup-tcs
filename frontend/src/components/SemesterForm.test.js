import React from 'react';
import {
  render,
  fireEvent,
  cleanup,
  wait
} from '../test-utils/CustomReactTestingLibrary';
import SemesterForm from './SemesterForm';

beforeEach(() => {
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      status: 200,
      headers: [],
      json: () => Promise.resolve([{ code: 1, name: '201901' }])
    })
  );
  global.fetch = fakeFetch;
});

test('Renders with required props', () => {
  const { container } = render(
    <SemesterForm title="Semester Test" onSubmit={jest.fn()} />
  );
  expect(container).toBeDefined();
});

test('Should have submit is disabled on rendered', () => {
  const { getByText } = render(
    <SemesterForm title="Semester Test" onSubmit={jest.fn()} />
  );
  expect(getByText(/submit/i)).toBeDisabled();
});

test('Should call onSubmit with semester values', async () => {
  const mockSubmit = jest.fn();
  const { getByText, getByTestId, findByText } = render(
    <SemesterForm title="Semester Test" onSubmit={mockSubmit} />
  );

  const semester = await findByText(/201901/);
  expect(semester).toBeDefined();
  fireEvent.change(getByTestId('semester-select'), { target: { value: '1' } });
  fireEvent.submit(getByText(/submit/i));

  await wait(() => {
    expect(mockSubmit).toHaveBeenCalledTimes(1);
    expect(mockSubmit).toHaveBeenCalledWith(
      { semesterCode: '1' },
      expect.anything()
    );
  });
});
