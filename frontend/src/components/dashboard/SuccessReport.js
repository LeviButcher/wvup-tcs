import React, { useState } from 'react';
import { CSVLink } from 'react-csv';
import styled from 'styled-components';
import { ReportLayout, Table, Header, Card, PieChart } from '../../ui';
import SemesterForm from '../SemesterForm';
import { callApi, ensureResponseCode, unwrapToJSON } from '../../utils';

const inDepartment = department => record =>
  department.departmentName === record.departmentName;

// return array of department Tuple (ID, Name)
const successToDepartmentReducer = (acc, curr) => {
  const department = acc.find(
    ele => ele.departmentName === curr.departmentName
  );
  if (department) return acc;
  acc.push({ departmentName: curr.departmentName, id: curr.departmentId });
  return acc;
};

// sum important columns for pie chart
const successReducer = (acc, curr, index) => {
  if (index === 0) {
    return { ...curr };
  }
  acc.uniqueStudentCount += curr.uniqueStudentCount;
  acc.droppedStudentCount += curr.droppedStudentCount;
  acc.completedCourseCount += curr.completedCourseCount;
  acc.passedSuccessfullyCount += curr.passedSuccessfullyCount;
  return acc;
};

const sumColumnsForPieChartsReducer = (acc, cur) => {
  const completed = acc.find(x => x.name === 'Completed') || {
    name: 'Completed',
    value: 0
  };
  const dropped = acc.find(x => x.name === 'Dropped') || {
    name: 'Dropped',
    value: 0
  };
  const passed = acc.find(x => x.name === 'Passed') || {
    name: 'Passed',
    value: 0
  };
  completed.value += cur.completedCourseCount;
  dropped.value += cur.droppedStudentCount;
  passed.value += cur.passedSuccessfullyCount;
  return [completed, dropped, passed];
};

const getSuccessData = semesterId =>
  callApi(`reports/success/${semesterId}`, 'GET', null);

const SuccessReport = () => {
  const [successRecords, setSuccessRecords] = useState();
  return (
    <ReportLayout>
      <SemesterForm
        name="Success Report"
        width="400px"
        onSubmit={({ semester }, { setSubmitting }) => {
          getSuccessData(semester)
            .then(ensureResponseCode(200))
            .then(unwrapToJSON)
            .then(setSuccessRecords)
            .finally(() => {
              setSubmitting(false);
            });
        }}
      />
      {successRecords &&
        successRecords
          .reduce(successToDepartmentReducer, [])
          .map(department => (
            <Card width="400px">
              <PieChart
                title={department.departmentName}
                data={successRecords
                  .filter(inDepartment(department))
                  .reduce(sumColumnsForPieChartsReducer, [])}
                x={x => x.name}
                y={y => y.value}
              />
            </Card>
          ))}
      {successRecords && (
        <Card width="1200px">
          <SuccessTable successRecords={successRecords} />
        </Card>
      )}
    </ReportLayout>
  );
};

const SuccessTable = ({ successRecords }) => {
  const sumOfAll = successRecords.reduce(successReducer, {});
  const sumRecord = (
    <tr align="left">
      <th>Total All:</th>
      <th></th>
      <th align="center">{sumOfAll.uniqueStudentCount}</th>
      <th align="center">{sumOfAll.droppedStudentCount}</th>
      <th align="center">{sumOfAll.completedCourseCount}</th>
      <th align="center">{sumOfAll.passedSuccessfullyCount}</th>
    </tr>
  );
  const allRows = successRecords
    .reduce(successToDepartmentReducer, [])
    .map(department => {
      const departmentData = successRecords.filter(inDepartment(department));
      const rows = departmentData.map(record => (
        <tr key={record.crn}>
          <td>{record.className}</td>
          <td>{record.crn}</td>
          <td align="center">{record.uniqueStudentCount}</td>
          <td align="center">{record.droppedStudentCount}</td>
          <td align="center">{record.completedCourseCount}</td>
          <td align="center">{record.passedSuccessfullyCount}</td>
        </tr>
      ));
      const sumDepartment = departmentData.reduce(successReducer, {});
      rows.push(
        <SpecialRow key={department.departmentName}>
          <td>Total {department.departmentName}:</td>
          <td></td>
          <td align="center">{sumDepartment.uniqueStudentCount}</td>
          <td align="center">{sumDepartment.droppedStudentCount}</td>
          <td align="center">{sumDepartment.completedCourseCount}</td>
          <td align="center">{sumDepartment.passedSuccessfullyCount}</td>
        </SpecialRow>
      );
      return rows;
    });

  return (
    <Table>
      <caption>
        <Header>
          SuccessReport -{' '}
          <CSVLink data={successRecords} filename="SuccessReport">
            Download
          </CSVLink>
        </Header>
        <p>
          Shows amount of students who visited the center and ended up passing
          the course
        </p>
      </caption>
      <thead>
        <tr align="left">
          <th>Course Name</th>
          <th>CRN</th>
          <th>Unique Students</th>
          <th>Dropped Course</th>
          <th title="Didn't drop out of course">Completed Course</th>
          <th title="Grade of C or higher">Passed Course</th>
        </tr>
      </thead>
      <tbody>{allRows}</tbody>
      <tfoot>{sumRecord}</tfoot>
    </Table>
  );
};

const SpecialRow = styled.tr`
  font-weight: 800;
`;

export default SuccessReport;
