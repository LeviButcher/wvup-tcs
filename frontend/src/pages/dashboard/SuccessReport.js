import React, { useState } from 'react';
import { CSVLink } from 'react-csv';
import styled from 'styled-components';
import { ReportLayout, Table, Header, Card, PieChart } from '../../ui';
import SemesterForm from '../../components/SemesterForm';
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
  const [pieChartData, setPieChartData] = useState();

  return (
    <ReportLayout>
      <SemesterForm
        style={{ gridArea: 'form' }}
        name="Success Report"
        width="500px"
        onSubmit={({ semester }, { setSubmitting }) => {
          getSuccessData(semester)
            .then(ensureResponseCode(200))
            .then(unwrapToJSON)
            .then(data => {
              setSuccessRecords(data);
              setPieChartData({ records: data, name: 'Total of Everything' });
            })
            .finally(() => {
              setSubmitting(false);
            });
        }}
      />
      {successRecords && (
        <Card width="1000px" style={{ gridArea: 'table' }}>
          <SuccessTable
            successRecords={successRecords}
            setPieChartData={setPieChartData}
          />
        </Card>
      )}
      {pieChartData && (
        <Card width="500px" style={{ gridArea: 'chart' }}>
          <PieChart
            title={pieChartData.name}
            data={pieChartData.records
              .reduce(sumColumnsForPieChartsReducer, [])
              .filter(x => x.value > 0)}
            x={x => x.name}
            y={y => y.value}
          />
        </Card>
      )}
    </ReportLayout>
  );
};

const SuccessTable = ({ successRecords, setPieChartData }) => {
  const sumOfAll = successRecords.reduce(successReducer, {});
  const sumRecord = (
    <SpecialRow
      onClick={() =>
        setPieChartData({
          records: successRecords,
          name: 'Total of Everything'
        })
      }
    >
      <th align="left">Total All:</th>
      <th></th>
      <th align="center">{sumOfAll.uniqueStudentCount}</th>
      <th align="center">{sumOfAll.droppedStudentCount}</th>
      <th align="center">{sumOfAll.completedCourseCount}</th>
      <th align="center">{sumOfAll.passedSuccessfullyCount}</th>
    </SpecialRow>
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
        <SpecialRow
          key={department.departmentName}
          onClick={() =>
            setPieChartData({
              records: departmentData,
              name: `${department.departmentName}`
            })
          }
        >
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
  &:hover {
    cursor: pointer;
  }
`;

export default SuccessReport;
