import React from 'react';
import {
  VictoryChart,
  VictoryBar,
  VictoryLabel,
  VictoryAxis,
  VictoryTheme,
  VictoryTooltip
} from 'victory';

const BarChart = ({ data, x, y, xLabel, yLabel, title, labels }) => (
  <VictoryChart theme={VictoryTheme.material} domainPadding={20}>
    <VictoryAxis
      label={yLabel}
      dependentAxis
      style={{ axisLabel: { padding: 40 } }}
    />
    <VictoryAxis
      label={xLabel}
      style={{ axisLabel: { padding: 40 }, tickLabels: { angle: -60 } }}
    />
    <VictoryLabel text={title} x="50%" y={30} textAnchor="middle" />
    <VictoryBar
      data={data}
      x={x}
      y={y}
      labels={labels}
      labelComponent={<VictoryTooltip />}
    />
  </VictoryChart>
);

export default BarChart;
