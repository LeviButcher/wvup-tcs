import React from 'react';
import {
  VictoryChart,
  VictoryBar,
  VictoryLabel,
  VictoryAxis,
  VictoryTheme,
  VictoryTooltip
} from 'victory';

type Props = {
  data: {},
  x: ({}) => string,
  y: ({}) => string,
  xLabel: string,
  yLabel: string,
  title: string,
  labels: string
};

const BarChart = ({
  data,
  x,
  y,
  xLabel,
  yLabel,
  title,
  labels,
  ...props
}: Props) => (
  <VictoryChart
    theme={VictoryTheme.material}
    domainPadding={20}
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
      fixLabelOverlap
      tickLabelComponent={<VictoryLabel renderInPortal />}
    />
    <VictoryLabel text={title} x={175} y={30} textAnchor="middle" />
    <VictoryBar
      data={data}
      x={x}
      y={y}
      labels={labels}
      labelComponent={<VictoryTooltip text={d => `${x(d)}: ${y(d)}`} />}
    />
  </VictoryChart>
);

export default BarChart;
