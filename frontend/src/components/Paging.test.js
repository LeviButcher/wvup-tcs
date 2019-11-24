import React from 'react';
import '@testing-library/jest-dom/extend-expect';
import { render, wait, fireEvent, cleanup } from '@testing-library/react';
import { Router, navigate } from '@reach/router';
import Paging from './Paging';

afterEach(cleanup);

it('Paging component renders with required props', () => {
  const { container } = render(
    <Paging basePath="" currentPage="3" totalPages="5" />
  );
  expect(container).toBeDefined();
});

it('Next is disabled when currentPage is equal to totalPages', () => {
  const { getByText } = render(
    <Paging basePath="/base" currentPage="5" totalPages="5" />
  );
  const next = getByText(/Next/);
  expect(next.getAttribute('aria-disabled')).toBeDefined();
});

it('Next is disabled when currentPage is greater than to totalPages', () => {
  const { getByText } = render(
    <Paging basePath="/base" currentPage="6" totalPages="5" />
  );
  const next = getByText(/Next/);
  expect(next.getAttribute('aria-disabled')).toBeDefined();
});

it('Prev is disabled when currentPage is equal to than to 1', () => {
  const { getByText } = render(
    <Paging basePath="/base" currentPage="1" totalPages="5" />
  );
  const prev = getByText(/Prev/);
  expect(prev.getAttribute('aria-disabled')).toBeDefined();
});

it('Prev is disabled when currentPage is less than 1', () => {
  const { getByText } = render(
    <Paging basePath="/base" currentPage="0" totalPages="5" />
  );
  const prev = getByText(/Prev/);
  expect(prev.getAttribute('aria-disabled')).toBeDefined();
});

it('Clicking on next changes url to be page2', async () => {
  const { getByText } = render(
    <Router>
      <Paging basePath="/base" currentPage="1" totalPages="5" default />
    </Router>
  );
  const next = getByText(/Next/);
  fireEvent.click(next);
  await wait(() => {
    expect(global.location.href).toContain('/base/2');
  });
});

it('Clicking on prev changes url to be page1', async () => {
  const { getByText } = render(
    <Router>
      <Paging basePath="/base" currentPage="2" totalPages="5" default />
    </Router>
  );
  const prev = getByText(/Prev/);
  fireEvent.click(prev);
  await wait(() => {
    expect(global.location.href).toContain('/base/1');
  });
});

it('Clicking on next changes url to be page3 and keeps queries', async () => {
  navigate('/base?max=10&min=20');
  const { getByText } = render(
    <Router>
      <Paging basePath="/base" currentPage="2" totalPages="5" default />
    </Router>
  );

  const next = getByText(/Next/);
  fireEvent.click(next);
  await wait(() => {
    expect(global.location.pathname).toEqual('/base/3');
    expect(global.location.search).toEqual('?max=10&min=20');
  });
});

it('Clicking on prev changes url to be page1 and keeps queries', async () => {
  navigate('/base?max=10&min=20');
  const { getByText } = render(
    <Router>
      <Paging basePath="/base" currentPage="2" totalPages="5" default />
    </Router>
  );

  const prev = getByText(/Prev/);
  fireEvent.click(prev);
  await wait(() => {
    expect(global.location.pathname).toEqual('/base/1');
    expect(global.location.search).toEqual('?max=10&min=20');
  });
});
