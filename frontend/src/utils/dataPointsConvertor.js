import { curry } from 'ramda';

const dataPointsConvertor = (xProperty, yProperty, object) => ({
  x: object[xProperty],
  y: object[yProperty]
});

// $FlowFixMe
export default curry(dataPointsConvertor);
