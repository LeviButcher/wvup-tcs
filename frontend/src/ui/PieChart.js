import React from 'react';
import { VictoryPie, VictoryTheme, VictoryLabel } from 'victory';

const PieChart = ({ data, x, y, title, ...props }) => (
  <svg viewBox="0 0 400 400">
    <VictoryPie
      width={400}
      height={400}
      standalone={false}
      data={data}
      x={x}
      y={y}
      theme={VictoryTheme.material}
      {...props}
    />
    <VictoryLabel text={title} x={200} y={30} textAnchor="middle" />
  </svg>
);

export default PieChart;
