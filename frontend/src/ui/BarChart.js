import React from 'react';
import {
  VictoryChart,
  VictoryBar,
  VictoryLabel,
  VictoryAxis,
  VictoryTheme,
  VictoryTooltip
} from 'victory';

const BarChart = ({ data, x, y, xLabel, yLabel, title, labels, ...props }) => (
  <VictoryChart
    theme={VictoryTheme.material}
    domainPadding={20}
    horizontal
    animate={{ duration: 1000 }}
    {...props}
  >
    <VictoryAxis
      label={yLabel}
      dependentAxis
      style={{ axisLabel: { padding: 40 } }}
    />
    <VictoryAxis
      label={xLabel}
      tickLabelComponent={<VictoryLabel renderInPortal />}
    />
    <VictoryLabel text={title} x={180} y={30} textAnchor="middle" />
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
