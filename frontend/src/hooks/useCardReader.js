import { useEffect, useReducer } from 'react';

const cardReaderStates = {
  read: 'read',
  parse: 'parse'
};

type DispatchType = {
  type: 'read' | 'parse',
  key?: string
};

// readingCard: true
// doneReadingCard: true
const cardReaderReducer = (currState, { type, key }: DispatchType) => {
  switch (type) {
    case cardReaderStates.read:
      return {
        ...currState,
        enteredValues: [...currState.enteredValues, key],
        doneReadingCard: key === 'Enter'
      };

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
            // $FlowFixMe
            acc[acc.length - 1] += curr;
            return acc;
          },
          ['']
        ),
        enteredValues: [],
        doneReadingCard: false
      };
    default:
      return currState;
  }
};

// Format of wvup id card: %{startofEmail}?;{wvupId}?
const useCardReader = () => {
  const [{ cardData, doneReadingCard }, dispatch] = useReducer(
    cardReaderReducer,
    {
      cardData: null,
      enteredValues: [],
      lastRead: null,
      doneReadingCard: false
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
    if (doneReadingCard === true) dispatch({ type: cardReaderStates.parse });
  }, [doneReadingCard]);

  return [cardData, doneReadingCard];
};

export default useCardReader;
