import { useEffect, useReducer } from 'react';
import { callApi, ensureResponseCode } from '../utils';

const loadingStates = {
  loading: 'Loading',
  done: 'Done',
  error: 'Error'
};

const getApiData = uri => callApi(uri, 'GET', null);

const loadingReducer = (currState, action) => {
  switch (action.type) {
    case loadingStates.loading: {
      return { ...currState, loading: true };
    }
    case loadingStates.done: {
      return { loading: false, data: action.data };
    }
    case loadingStates.error: {
      return { loading: false, data: action.errors };
    }
    default: {
      return currState;
    }
  }
};

const useApiWithHeaders = uri => {
  const [{ loading, errors, data }, dispatch] = useReducer(loadingReducer, {
    loading: true,
    data: {
      headers: {},
      body: []
    }
  });

  useEffect(() => {
    if (uri === null || uri.length < 1) {
      dispatch({ type: loadingStates.done });
      return;
    }
    dispatch({ type: loadingStates.loading });
    getApiData(uri)
      .then(ensureResponseCode(200))
      .then(async response => {
        const buildData = { headers: {} };
        response.headers.forEach((value, key) => {
          buildData.headers[key] = value;
        });
        buildData.body = await response.json();
        dispatch({ type: loadingStates.done, data: buildData });
      })
      .catch(e => {
        dispatch({ type: loadingStates.error, errors: e });
      });
  }, [uri]);

  return [loading, data, errors];
};

export default useApiWithHeaders;
