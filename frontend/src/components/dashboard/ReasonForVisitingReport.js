import React, { useState } from 'react';
import { RadialChart } from 'react-vis';
import { CSVLink } from 'react-csv';
import { clone } from 'ramda';
import { ReportLayout, Table, Header, Card } from '../../ui';
import StartToEndDateForm from '../StartToEndDateForm';
import callApi from '../../utils/callApi';
import ensureResponseCode from '../../utils/ensureResponseCode';
import unwrapToJSON from '../../utils/unwrapToJSON';

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
  callApi(
    `${process.env.REACT_APP_BACKEND}reports/reasons?start=${startDate}&end=${endDate}`,
    'GET',
    null
  );

const ReasonsReport = () => {
  const [reasonsForVisiting, setReasonsForVisiting] = useState();
  return (
    <ReportLayout>
      <div>
        <StartToEndDateForm
          onSubmit={({ startDate, endDate }, { setSubmitting }) => {
            getReasons(startDate, endDate)
              .then(ensureResponseCode(200))
              .then(unwrapToJSON)
              .then(setReasonsForVisiting)
              .finally(() => setSubmitting(false));
          }}
          name="Reason For Visiting"
        />
        {reasonsForVisiting && (
          <Card width="fit-content">
            <RadialChart
              data={reasonsForVisiting
                .reduce(reasonTotalStudentReducer, [])
                .map(reasonsToAngle)}
              width={400}
              height={400}
              showLabels
              labelsStyle={{ fontSize: 16, fill: '#222' }}
              labelsRadiusMultiplier={1.1}
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
