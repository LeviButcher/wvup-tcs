const loadingStates = {
  loading: 'Loading',
  done: 'Done',
  error: 'Error'
};

type LoadingState = {
  loading: boolean,
  data: any,
  errors: any
};

type Action = {
  type: 'Loading' | 'Done' | 'Error',
  data: any,
  errors: any
};

const loadingReducer = (currState: LoadingState, action: Action) => {
  switch (action.type) {
    case loadingStates.loading: {
      return { ...currState, loading: true };
    }
    case loadingStates.done: {
      return { ...currState, loading: false, data: action.data };
    }
    case loadingStates.error: {
      return { ...currState, loading: false, errors: action.errors };
    }
    default: {
      return currState;
    }
  }
};

export { loadingStates, loadingReducer };
