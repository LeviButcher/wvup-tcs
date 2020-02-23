import React from 'react';
import { CSVLink } from 'react-csv';
import { Router } from '@reach/router';
import { ReportLayout, Table, Header, Card, PieChart } from '../../ui';
import StartToEndDateForm from '../../components/StartToEndDateForm';
import useApi from '../../hooks/useApi';

type ReasonWithClassVisits = {
  reasonName: string,
  reasonId: string,
  className: string,
  classCRN: string,
  visits: number
};

const uniqueElementsReducer = func => (acc, curr) => {
  const found = acc.find(func(curr));
  if (found) return acc;
  return [...acc, curr];
};

const filterReason = reasonName => element => element.reasonName === reasonName;

// Takes a list of ReasonWithClassVisits and returns a list of only the unique reason names
const uniqueReasonNamesReducer = uniqueElementsReducer(reasonName => element =>
  element === reasonName
);

// Used to get a Reasons Total Visits, for Pie Chart
const sumUniqueReasonsWithClassVisits = reasons =>
  reasons
    .map(x => x.reasonName)
    .reduce(uniqueReasonNamesReducer, [])
    .map(reason => ({
      reasonName: reason,
      visits: reasons
        .filter(filterReason(reason))
        .reduce((acc, r) => acc + r.visits, 0)
    }));

type Props = {
  navigate: any,
  '*': string
};

const ReasonsReport = ({ navigate, '*': unMatchedUri }: Props) => {
  const [start, end] = unMatchedUri.split('/');
  return (
    <ReportLayout>
      <StartToEndDateForm
        title="Reason For Visiting Report"
        style={{ gridArea: 'form' }}
        initialValues={{
          startDate: start || '',
          endDate: end || ''
        }}
        onSubmit={({ startDate, endDate }) => {
          return Promise.resolve(navigate(`${startDate}/${endDate}`));
        }}
      />
      <Router primary={false} component={({ children }) => <>{children}</>}>
        {/* $FlowFixMe */}
        <ReasonsResult path=":startDate/:endDate" />
      </Router>
    </ReportLayout>
  );
};

type ReasonsResultProps = {
  startDate: string,
  endDate: string
};

const ReasonsResult = ({ startDate, endDate }: ReasonsResultProps) => {
  const [loading, reasonData] = useApi(
    `reports/reasons?start=${startDate}&end=${endDate}`
  );
  return (
    <>
      {loading && <div>Loading...</div>}
      {!loading && reasonData && (
        <>
          <Card width="600px" style={{ gridArea: 'chart' }}>
            <PieChart
              title="Reason For Visiting Percentages"
              data={sumUniqueReasonsWithClassVisits(reasonData)}
              x={d => d.reasonName}
              y={d => d.visits}
            />
          </Card>
          <Card width="900px" style={{ gridArea: 'table' }}>
            <Header align="center">
              Reason for Visiting Summary -{' '}
              <CSVLink data={reasonData} filename="reasonForVisiting.csv">
                Download All Data
              </CSVLink>
            </Header>
            {reasonData
              .map(x => x.reasonName)
              .reduce(uniqueReasonNamesReducer, [])
              .map(reason => (
                <ReasonsTable
                  key={reason}
                  name={reason}
                  reasons={reasonData.filter(filterReason(reason))}
                />
              ))}
          </Card>
        </>
      )}
    </>
  );
};

type ReasonsTableProps = {
  reasons: Array<ReasonWithClassVisits>,
  name: string
};

const ReasonsTable = ({ reasons, name }: ReasonsTableProps) => {
  return (
    <Table>
      <caption>
        <Header type="h3">
          {name} -{' '}
          <CSVLink data={reasons} filename={`${name}.csv`}>
            Download
          </CSVLink>
        </Header>
      </caption>
      <thead align="left">
        <tr>
          <th>Course</th>
          <th>CRN</th>
          <th>Total Students Visited</th>
        </tr>
      </thead>
      <tbody>
        {reasons.map(reason => (
          <tr key={reason.className + reason.reasonName}>
            <td>{reason.className}</td>
            <td>{reason.classCRN}</td>
            <td>{reason.visits}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default ReasonsReport;
