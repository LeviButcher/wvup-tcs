import React, { useState } from 'react';
import { CSVLink } from 'react-csv';
import { clone } from 'ramda';
import { ReportLayout, Table, Header, Card, PieChart } from '../../ui';
import StartToEndDateForm from '../StartToEndDateForm';
import {
  callApi,
  ensureResponseCode,
  unwrapToJSON,
  errorToMessage
} from '../../utils';

// take all reasons and split up into reason groups
const filterReason = reason => element => element.reasonName === reason;

// reducer to give reason, filter by reason
const reasonForVisitingToReasonsReducer = (acc, reasonForVisiting) => {
  const found = acc.find(element => element === reasonForVisiting.reasonName);
  if (found) return acc;
  acc.push(reasonForVisiting.reasonName);
  return acc;
};

const reasonTotalStudentReducer = (acc, reason) => {
  // reason already in acc, add to totalStudents
  const index = acc.findIndex(filterReason(reason.reasonName));
  if (index !== -1) {
    acc[index].visits += reason.visits;
  } else {
    acc.push(clone(reason));
  }
  return acc;
};

const reasonsToAngle = reason => ({
  angle: reason.visits,
  label: reason.reasonName
});

const getReasons = (startDate, endDate) =>
  callApi(`reports/reasons?start=${startDate}&end=${endDate}`, 'GET', null);

const ReasonsReport = () => {
  const [reasonsForVisiting, setReasonsForVisiting] = useState();
  return (
    <ReportLayout>
      <div>
        <StartToEndDateForm
          onSubmit={({ startDate, endDate }, { setSubmitting, setStatus }) => {
            getReasons(startDate, endDate)
              .then(ensureResponseCode(200))
              .then(unwrapToJSON)
              .then(setReasonsForVisiting)
              .catch(errorToMessage)
              .then(setStatus)
              .finally(() => setSubmitting(false));
          }}
          name="Reason For Visiting"
        />
        {reasonsForVisiting && (
          <Card width="600px" padding={0}>
            <PieChart
              data={reasonsForVisiting
                .reduce(reasonTotalStudentReducer, [])
                .map(reasonsToAngle)}
              x={d => d.label}
              y={d => d.angle}
              labels={d => `${d.label}: ${d.angle}`}
              title="Reason For Visiting Percentages"
              padding={80}
            />
          </Card>
        )}
      </div>
      <div>
        {reasonsForVisiting && (
          <>
            <Header align="center">
              Reason for Visiting Summary -{' '}
              <CSVLink data={reasonsForVisiting} filename="reasonForVisiting">
                Download All Data
              </CSVLink>
            </Header>
            {reasonsForVisiting
              .reduce(reasonForVisitingToReasonsReducer, [])
              .map(reason => (
                <ReasonsTable
                  key={reason}
                  name={reason}
                  reasons={reasonsForVisiting.filter(filterReason(reason))}
                />
              ))}
          </>
        )}
      </div>
    </ReportLayout>
  );
};

const ReasonsTable = ({ reasons, name }) => {
  return (
    <Table>
      <caption>
        <Header type="h3">
          {name} -{' '}
          <CSVLink data={reasons} filename={`${name}`}>
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
          <tr key={reason.courseName + reason.reasonName}>
            <td>{reason.courseName}</td>
            <td>{reason.courseCRN}</td>
            <td>{reason.visits}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default ReasonsReport;
