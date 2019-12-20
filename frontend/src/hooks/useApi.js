import { useReducer, useEffect } from 'react';
import { apiFetch } from '../utils/fetchLight';

// $FlowFixMe
const apiReducer = (currState, { type, data = null, error = null }) => {
  switch (type) {
    case 'loading':
      return { loading: true, data: null, error: null };
    case 'done':
      return { loading: false, data, error: null };
    case 'error':
      return { loading: false, error, data: null };
    default:
      return currState;
  }
};

const useApi = (url: string) => {
  const [{ loading, data, error }, dispatch] = useReducer(apiReducer, {
    loading: false,
    data: null,
    error: null
  });

  useEffect(() => {
    let isMounted = true;

    dispatch({ type: 'loading' });
    apiFetch(url, 'GET', null)
      .then(res => res.json())
      .then(jsonData => isMounted && dispatch({ type: 'done', data: jsonData }))
      .catch(
        thrownError =>
          isMounted && dispatch({ type: 'error', error: thrownError })
      );

    return () => {
      isMounted = false;
    };
  }, [url]);

  return [loading, data, error];
};

export default useApi;
