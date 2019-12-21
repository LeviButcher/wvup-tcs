import fetchLight from './fetchLight';
import makeFetchReturn from '../test-utils/makeFetchReturn';

it('Always passes credential and content type json header to fetch', () => {
  const mockFetch = jest.fn(() => Promise.resolve({}));
  global.fetch = mockFetch;
  fetchLight('/whatever', 'GET', {});

  expect(mockFetch).toHaveBeenCalledWith('/whatever', {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json'
    },
    body: null
  });
});

it('Changes method to POST when method passed in is POST and stringifies body', () => {
  const mockFetch = jest.fn(() => Promise.resolve({}));
  global.fetch = mockFetch;
  fetchLight('/fake', 'POST', { fake: 'data' });

  expect(mockFetch).toHaveBeenCalledWith('/fake', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: '{"fake":"data"}'
  });
});

it('fetch returns 200, returns back the response', async () => {
  makeFetchReturn({ status: 200 })({});
  const response = await fetchLight('/fake', 'POST', { fake: 'data' });

  expect(response.status).toBe(200);
});

describe('failedFetch throws on bad status codes', () => {
  const statusCodesShouldThrowFailedRequestError = [100, 300, 400, 500];

  statusCodesShouldThrowFailedRequestError.forEach(status => {
    it(`fetch returns ${status}, throws FailedRequestError`, () => {
      expect.assertions(2);
      makeFetchReturn({ status })({});

      return fetchLight('/fail', 'GET', {}).catch(e => {
        expect(e.message).toBe(`Failed Request | status code: ${status}`);
        expect(e.response.status).toBe(status);
      });
    });
  });
});
