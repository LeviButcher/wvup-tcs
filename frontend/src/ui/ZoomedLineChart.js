import React, { useState } from 'react';
import {
  VictoryLine,
  VictoryChart,
  VictoryTooltip,
  VictoryAxis,
  VictoryLabel,
  VictoryTheme,
  VictoryScatter,
  createContainer
} from 'victory';
import { last, minBy, maxBy, reduce, identity } from 'ramda';

const VictoryZoomVoronoiContainer = createContainer('zoom', 'voronoi');

const getEntireDomain = (x, y, data) => ({
  y: [
    reduce(minBy(identity), Infinity, data.map(y)),
    reduce(maxBy(identity), -Infinity, data.map(y))
  ],
  x: [x(data[0]), x(last(data))]
});

const LineChart = ({ data, x, y, xLabel, yLabel, title, labels, ...props }) => {
  const [entireDomain] = useState(getEntireDomain(x, y, data));
  const [zoomedXDomain, setZoomX] = useState(entireDomain.x);

  const getVisableData = () =>
    data.filter(d => x(d) >= zoomedXDomain[0] && x(d) <= zoomedXDomain[1]);

  return (
    <VictoryChart
      theme={VictoryTheme.material}
      domain={entireDomain}
      animate={{ duration: 1000 }}
      containerComponent={
        <VictoryZoomVoronoiContainer
          zoomDimension="x"
          onZoomDomainChange={domain => setZoomX(domain.x)}
        />
      }
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
        data={getVisableData()}
        x={x}
        y={y}
        theme={VictoryTheme.material}
        labels={labels}
        labelComponent={<VictoryTooltip text={d => `${x(d)}: ${y(d)}`} />}
        {...props}
      />
      <VictoryScatter data={getVisableData()} x={x} y={y} />
    </VictoryChart>
  );
};

export default LineChart;
