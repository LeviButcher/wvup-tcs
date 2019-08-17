const loadingStates = {
  loading: 'Loading',
  done: 'Done',
  error: 'Error'
};

const loadingReducer = (currState, action) => {
  switch (action.type) {
    case loadingStates.loading: {
      return { ...currState, loading: true };
    }
    case loadingStates.done: {
      return { loading: false, data: action.data };
    }
    case loadingStates.error: {
      return { loading: false, errors: action.errors };
    }
    default: {
      return currState;
    }
  }
};

export { loadingStates, loadingReducer };
