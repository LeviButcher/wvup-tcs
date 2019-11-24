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
    },
    errors: {}
  });

  useEffect(() => {
    let isMounted = true;
    if (uri === null || uri.length < 1) {
      if (isMounted)
        dispatch({
          type: loadingStates.done,
          data: { headers: {}, body: [] },
          errors: {}
        });
      return () => {
        isMounted = false;
      };
    }
    if (isMounted)
      dispatch({
        type: loadingStates.loading,
        data: {
          headers: {},
          body: { headers: {}, body: [] }
        },
        errors: {}
      });
    getApiData(uri)
      .then(ensureResponseCode(200))
      .then(async response => {
        const buildData = { headers: {}, body: {} };
        response.headers.forEach((value, key) => {
          buildData.headers[key] = value;
        });
        buildData.body = await response.json();
        if (isMounted) {
          dispatch({ type: loadingStates.done, data: buildData, errors: {} });
        }
      })
      .catch(e => {
        if (isMounted) {
          dispatch({
            type: loadingStates.error,
            errors: e,
            data: {
              headers: {},
              body: []
            }
          });
        }
      });

    return () => {
      isMounted = false;
    };
  }, [uri]);

  return [loading, data, errors];
};

export default useApiWithHeaders;
