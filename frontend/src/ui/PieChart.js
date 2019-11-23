import React from 'react';
import { VictoryPie, VictoryTheme, VictoryLabel } from 'victory';
import { add, sum } from 'ramda';

type LabelWithPercentageProps = {
  text: string,
  data: {},
  datum: string
};

const LabelWithPercentage = ({
  text,
  data,
  datum,
  ...props
}: LabelWithPercentageProps) => {
  return (
    <g>
      <VictoryLabel
        {...props}
        text={() => {
          return `${text}\n${(
            (datum._y / sum(data.map(da => da._y))) *
            100
          ).toFixed(1)}%`;
        }}
      />
    </g>
  );
};

type Props = {
  data: Array<Object>,
  x: ({}) => string,
  y: ({}) => string,
  title: string
};

const PieChart = ({ data, x, y, title, ...props }: Props) => (
  <svg viewBox="0 0 500 500">
    <VictoryPie
      padding={100}
      colorScale="blue"
      animate={{ duration: 500 }}
      width={500}
      height={500}
      standalone={false}
      data={data}
      x={x}
      y={y}
      theme={VictoryTheme.material}
      style={{ labels: { fill: 'black' } }}
      labelComponent={LabelWithPercentage}
      {...props}
    />
    <VictoryLabel
      text={title}
      x={250}
      y={20}
      textAnchor="middle"
      style={{ fontSize: '20px' }}
    />
    <text x={250} y={500} textAnchor="middle">
      Total Students:{data.map(d => y(d)).reduce(add, 0)}
    </text>
  </svg>
);

export default PieChart;
