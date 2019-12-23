import { curry } from 'ramda';

const backendURL = process.env.REACT_APP_BACKEND || '';

type HTTPMethod = 'POST' | 'GET' | 'PUT' | 'DELETE';

function callApi(uri: string, method: HTTPMethod, data: {}) {
  console.warn('Deprecating callApi eventually, use fetchLight');
  const token = localStorage.getItem(`${process.env.REACT_APP_TOKEN || ''}`);
  const options = {
    method,
    headers: {
      'Content-Type': 'application/json'
    }
  };
  if (token) {
    // $FlowFixMe
    options.headers.Authorization = `Bearer ${token}`;
  }

  if (data) {
    // $FlowFixMe
    options.body = JSON.stringify(data);
  }

  return fetch(`${backendURL}${uri}`, options);
}

// $FlowFixMe
export default curry(callApi);
