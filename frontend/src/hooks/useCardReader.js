import { useEffect, useReducer } from 'react';

const cardReaderStates = {
  read: 'read',
  setTime: 'setTime',
  reset: 'reset'
};

const cardReaderReducer = (currState, { type, key }) => {
  switch (type) {
    case cardReaderStates.read:
      return {
        ...currState,
        enteredValues: [...currState.enteredValues, key]
      };
    case cardReaderStates.setTime:
      return { ...currState, lastRead: new Date() };
    case cardReaderStates.parse:
      return {
        ...currState,
        cardData: currState.enteredValues.reduce(
          (acc, curr) => {
            if (curr === '?') {
              acc.push('');
              return acc;
            }
            if (curr === ';' || curr === 'Enter') return acc;
            acc[acc.length - 1] += curr;
            return acc;
          },
          ['']
        )
      };
    case cardReaderStates.reset:
      return { cardData: null, enteredValues: [], lastRead: null };
    default:
      return currState;
  }
};

const useCardReader = () => {
  const [{ cardData, enteredValues, lastRead }, dispatch] = useReducer(
    cardReaderReducer,
    {
      cardData: null,
      enteredValues: [],
      lastRead: null
    }
  );
  const readKeys = ({ key }) => {
    dispatch({ type: cardReaderStates.read, key });
  };

  useEffect(() => {
    window.addEventListener('keypress', readKeys);
    return () => window.removeEventListener('keypress', readKeys);
  }, []);

  useEffect(() => {
    const currTime = new Date();
    if (currTime - lastRead < 100) {
      dispatch({ type: cardReaderStates.parse });
    } else {
      dispatch({ type: cardReaderStates.reset });
    }
    dispatch({ type: cardReaderStates.setTime });
  }, [enteredValues]);

  return [cardData];
};

export default useCardReader;
