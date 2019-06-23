import { curry } from 'ramda';

function callApi(url, method, data) {
  const options = {
    method,
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${localStorage.getItem(
        `${process.env.REACT_APP_TOKEN}`
      )}`
    }
  };
  if (data) {
    options.body = JSON.stringify(data);
  }

  return fetch(url, options);
}

export default curry(callApi);
