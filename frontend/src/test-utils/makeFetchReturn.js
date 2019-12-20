import { curry } from 'ramda';

const makeFetchReturn = (props: { status: string }, toReturn: any) => {
  const fakeFetch = jest.fn(() =>
    Promise.resolve({
      json: () => Promise.resolve(toReturn),
      ...props
    })
  );
  global.fetch = fakeFetch;
  return fakeFetch;
};

// $FlowFixMe
export default curry(makeFetchReturn);
