import React from 'react';
import {
  VictoryLine,
  VictoryChart,
  VictoryTooltip,
  VictoryAxis,
  VictoryLabel,
  VictoryTheme,
  VictoryVoronoiContainer,
  VictoryScatter
} from 'victory';

const LineChart = ({ data, x, y, xLabel, yLabel, title, labels, ...props }) => (
  <VictoryChart
    theme={VictoryTheme.material}
    containerComponent={<VictoryVoronoiContainer />}
    animate={{ duration: 1000 }}
  >
    <VictoryLabel text={title} x={180} y={30} textAnchor="middle" />
    <VictoryAxis
      label={yLabel}
      dependentAxis
      style={{ axisLabel: { padding: 40 } }}
    />
    <VictoryAxis
      label={xLabel}
      style={{ axisLabel: { padding: 40 } }}
      fixLabelOverlap
    />
    <VictoryLine
      data={data}
      x={x}
      y={y}
      theme={VictoryTheme.material}
      labels={labels}
      labelComponent={<VictoryTooltip text={d => `${x(d)}: ${y(d)}`} />}
      {...props}
    />
    <VictoryScatter data={data} x={x} y={y} />
  </VictoryChart>
);

export default LineChart;
