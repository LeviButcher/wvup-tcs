import React, { useState } from 'react';
import { RadialChart } from 'react-vis';
import { CSVLink } from 'react-csv';
import { clone } from 'ramda';
import { ReportLayout, Table, Header, Card } from '../../ui';
import StartToEndDateForm from '../StartToEndDateForm';

import makeAsync from '../../utils/makeAsync';

const reasonForVisitingData = [
  {
    course: 'Math 126',
    CRN: '11554',
    totalStudents: 29,
    reason: 'Computer Use'
  },
  {
    course: 'English 102',
    CRN: '11555',
    totalStudents: 50,
    reason: 'Computer Use'
  },
  {
    course: 'Physics 101',
    CRN: '11556',
    totalStudents: 18,
    reason: 'Computer Use'
  },
  {
    course: 'No Class Given',
    totalStudents: 18,
    reason: 'Computer Use'
  },
  {
    course: 'Math 126',
    CRN: '11554',
    totalStudents: 20,
    reason: 'Study Time'
  },
  {
    course: 'English 102',
    CRN: '11555',
    totalStudents: 13,
    reason: 'Study Time'
  },
  {
    course: 'Physics 101',
    CRN: '11556',
    totalStudents: 12,
    reason: 'Study Time'
  },
  {
    course: 'No Class Given',
    totalStudents: 2,
    reason: 'Study Time'
  },
  {
    course: 'CS 122',
    CRN: '11554',
    totalStudents: 20,
    reason: 'Written Paper Review'
  },
  {
    course: 'English 102',
    CRN: '11555',
    totalStudents: 13,
    reason: 'Written Paper Review'
  },
  {
    course: 'Physics 101',
    CRN: '11556',
    totalStudents: 12,
    reason: 'Written Paper Review'
  },
  {
    course: 'No Class Given',
    totalStudents: 2,
    reason: 'Written Paper Review'
  }
];
// take all reasons and split up into reason groups

const filterReason = reason => element => element.reason === reason;

// reducer to give reason, filter by reason
const reasonForVisitingToReasonsReducer = (acc, reasonForVisiting) => {
  const found = acc.find(element => element === reasonForVisiting.reason);
  if (found) return acc;
  acc.push(reasonForVisiting.reason);
  return acc;
};

const reasonTotalStudentReducer = (acc, reason) => {
  // reason already in acc, add to totalStudents
  const index = acc.findIndex(filterReason(reason.reason));
  if (index !== -1) {
    acc[index].totalStudents += reason.totalStudents;
  } else {
    acc.push(clone(reason));
  }
  return acc;
};

const reasonsToAngle = reason => ({
  angle: reason.totalStudents,
  label: reason.reason
});

const getReasons = makeAsync(1000, reasonForVisitingData);

const ReasonsReport = () => {
  const [reasonsForVisiting, setReasonsForVisiting] = useState();
  return (
    <ReportLayout>
      <div>
        <StartToEndDateForm
          onSubmit={(values, { setSubmitting }) => {
            getReasons().then(res => {
              setReasonsForVisiting(res);
              setSubmitting(false);
            });
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
      <thead>
        <tr>
          <td>Course</td>
          <td>CRN</td>
          <td>Total Students Visited</td>
        </tr>
      </thead>
      <tbody>
        {reasons.map(reason => (
          <tr key={reason.week}>
            <td>{reason.course}</td>
            <td>{reason.CRN}</td>
            <td>{reason.totalStudents}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default ReasonsReport;
