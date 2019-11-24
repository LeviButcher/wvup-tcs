import { useReducer } from 'react';
import { loadingReducer, loadingStates } from './loadingReducer';

const useLoading = () => {
  const [{ loading }, dispatch] = useReducer(loadingReducer, {
    loading: false,
    data: {},
    errors: {}
  });

  const startLoading = () =>
    dispatch({ type: loadingStates.loading, data: {}, errors: {} });
  const finishLoading = () =>
    dispatch({ type: loadingStates.done, data: {}, errors: {} });
  return [loading, startLoading, finishLoading];
};

export default useLoading;
