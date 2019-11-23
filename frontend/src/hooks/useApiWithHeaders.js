import { useEffect, useReducer } from 'react';
import { callApi, ensureResponseCode } from '../utils';
import { loadingReducer, loadingStates } from './loadingReducer';

const getApiData = uri => callApi(uri, 'GET', null);

const useApiWithHeaders = (uri: string) => {
  const [{ loading, errors, data }, dispatch] = useReducer(loadingReducer, {
    loading: true,
    data: {
      headers: {},
      body: []
    }
  });

  useEffect(() => {
    let isMounted = true;
    if (uri === null || uri.length < 1) {
      if (isMounted) dispatch({ type: loadingStates.done });
      return () => {
        isMounted = false;
      };
    }
    if (isMounted) dispatch({ type: loadingStates.loading });
    getApiData(uri)
      .then(ensureResponseCode(200))
      .then(async response => {
        const buildData = { headers: {} };
        response.headers.forEach((value, key) => {
          buildData.headers[key] = value;
        });
        buildData.body = await response.json();
        if (isMounted) dispatch({ type: loadingStates.done, data: buildData });
      })
      .catch(e => {
        if (isMounted) dispatch({ type: loadingStates.error, errors: e });
      });

    return () => {
      isMounted = false;
    };
  }, [uri]);

  return [loading, data, errors];
};

export default useApiWithHeaders;
