import { useReducer } from 'react';
import { loadingReducer, loadingStates } from './loadingReducer';

const useLoading = () => {
  const [{ loading }, dispatch] = useReducer(loadingReducer, {
    loading: false
  });

  const startLoading = () => dispatch({ type: loadingStates.loading });
  const finishLoading = () => dispatch({ type: loadingStates.done });
  return [loading, startLoading, finishLoading];
};

export default useLoading;
