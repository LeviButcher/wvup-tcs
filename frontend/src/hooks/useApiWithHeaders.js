import { useEffect, useReducer } from 'react';
import { callApi, ensureResponseCode } from '../utils';
import { loadingReducer, loadingStates } from './loadingReducer';

const getApiData = uri => callApi(uri, 'GET', null);

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
        console.log(response);
        response.headers.forEach((value, key) => {
          buildData.headers[key] = value;
        });
        console.log(buildData.headers);
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
