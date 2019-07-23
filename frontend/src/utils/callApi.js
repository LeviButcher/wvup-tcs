import { curry } from 'ramda';

function callApi(uri, method, data) {
  const options = {
    method,
    headers: {
      'Content-Type': 'application/json',
      'Access-Control-Expose-Headers': true,
      'Access-Control-Allow-Headers': true,
      Authorization: `Bearer ${localStorage.getItem(
        `${process.env.REACT_APP_TOKEN}`
      )}`
    }
  };
  if (data) {
    options.body = JSON.stringify(data);
  }

  return fetch(`${process.env.REACT_APP_BACKEND}${uri}`, options);
}

export default curry(callApi);
