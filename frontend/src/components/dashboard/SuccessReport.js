import React, { useState } from 'react';
import { CSVLink } from 'react-csv';
import styled from 'styled-components';
import { ReportLayout, Table, Header } from '../../ui';
import SemesterForm from '../SemesterForm';

const successData = [
  {
    departmentId: '1',
    departmentName: 'Math',
    courseName: 'Math 121',
    CRN: '15142',
    uniqueStudents: 50,
    droppedStudents: 5,
    completedCourse: 45,
    cOrHigherStudents: 20
  },
  {
    departmentId: '1',
    departmentName: 'Math',
    courseName: 'Math 126',
    CRN: '15482',
    uniqueStudents: 43,
    droppedStudents: 3,
    completedCourse: 40,
    cOrHigherStudents: 25
  },
  {
    departmentId: '1',
    departmentName: 'Math',
    courseName: 'Math 211',
    CRN: '15482',
    uniqueStudents: 26,
    droppedStudents: 6,
    completedCourse: 20,
    cOrHigherStudents: 15
  },
  {
    departmentId: '2',
    departmentName: 'English',
    courseName: 'Eng 101',
    CRN: '15484',
    uniqueStudents: 16,
    droppedStudents: 6,
    completedCourse: 10,
    cOrHigherStudents: 10
  },
  {
    departmentId: '2',
    departmentName: 'English',
    courseName: 'Eng 102',
    CRN: '15485',
    uniqueStudents: 42,
    droppedStudents: 2,
    completedCourse: 40,
    cOrHigherStudents: 26
  }
];

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

// sum important columns
const successReducer = (acc, curr, index) => {
  if (index === 0) {
    return { ...curr };
  }
  acc.uniqueStudents += curr.uniqueStudents;
  acc.droppedStudents += curr.droppedStudents;
  acc.completedCourse += curr.completedCourse;
  acc.cOrHigherStudents += curr.cOrHigherStudents;
  return acc;
};

const getSuccessData = () => Promise.resolve(successData);

const SuccessReport = () => {
  const [successRecords, setSuccessRecords] = useState();

  return (
    <ReportLayout>
      <div>
        <SemesterForm
          name="Success"
          width="400px"
          onSubmit={(values, { setSubmitting }) => {
            getSuccessData().then(res => {
              setSuccessRecords(res);
              setSubmitting(false);
            });
          }}
        />
      </div>
      {successRecords && <SuccessTable successRecords={successRecords} />}
    </ReportLayout>
  );
};

const SuccessTable = ({ successRecords }) => {
  const sumOfAll = successRecords.reduce(successReducer, {});
  const sumRecord = (
    <tr align="left">
      <th>Total All:</th>
      <th></th>
      <th align="center">{sumOfAll.uniqueStudents}</th>
      <th align="center">{sumOfAll.droppedStudents}</th>
      <th align="center">{sumOfAll.completedCourse}</th>
      <th align="center">{sumOfAll.cOrHigherStudents}</th>
    </tr>
  );
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
          <th>Completed Course</th>
          <th>Passed Course (C or higher)</th>
        </tr>
      </thead>
      <tbody>
        {successRecords
          .reduce(successToDepartmentReducer, [])
          .map(department => {
            const departmentData = successRecords.filter(
              inDepartment(department)
            );
            const rows = departmentData.map(record => (
              <tr>
                <td>{record.courseName}</td>
                <td>{record.CRN}</td>
                <td align="center">{record.uniqueStudents}</td>
                <td align="center">{record.droppedStudents}</td>
                <td align="center">{record.completedCourse}</td>
                <td align="center">{record.cOrHigherStudents}</td>
              </tr>
            ));
            const sumDepartment = departmentData.reduce(successReducer, {});
            rows.push(
              <SpecialRow>
                <td>Total {department.departmentName}:</td>
                <td></td>
                <td align="center">{sumDepartment.uniqueStudents}</td>
                <td align="center">{sumDepartment.droppedStudents}</td>
                <td align="center">{sumDepartment.completedCourse}</td>
                <td align="center">{sumDepartment.cOrHigherStudents}</td>
              </SpecialRow>
            );
            return rows;
          })}
      </tbody>
      <tfoot>{sumRecord}</tfoot>
    </Table>
  );
};

const SpecialRow = styled.tr`
  background: ${props => props.theme.color.accent}88 !important;
  font-weight: bold;
`;

// Group by Department, Display Summation of Department records,  Display total at end

export default SuccessReport;
