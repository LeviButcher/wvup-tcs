import { curry } from 'ramda';

const makeAsync = (time, data) => {
  return () =>
    new Promise(res => {
      setTimeout(() => {
        res(data);
      }, time);
    });
};

export default curry(makeAsync);
