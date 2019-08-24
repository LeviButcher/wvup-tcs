import React from 'react';
import { Table, Link } from '../ui';
import { Gear, Trashcan } from '../ui/icons';

const ClassToursTable = ({ classTours }) => {
  return(
    <Table>
      <thead align="left">
        <tr>
          <th align="center">Name</th>
          <th align="center">Day Visited</th>
          <th align="center">Number of Students</th>
          <th align="center" >Actions</th>
        </tr>
      </thead>
      <tbody>
        {classTours.map(s => (
          <ClassTourRow classTour={s} key={s.id} />
        ))}
      </tbody>
    </Table>
  );
};

const dateOptions = {
  year: '2-digit',
  month: 'numeric',
  day: 'numeric',
  hourCycle: 'h12',
  hour: '2-digit',
  minute: '2-digit'
};

const ClassTourRow = ({
  classTour: {
    id,
    name,
    dayVisited,
    numberOfStudents
  }
}) => {
  return (
    <tr>
      <td>{name}</td>
      <td align="center">
        {new Date(dayVisited).toLocaleDateString(undefined, dateOptions)}
      </td>
      <td align="center">{numberOfStudents}</td>
      <td style={{ display: 'flex', justifyContent: 'space-evenly' }}>
        <Link to={`/dashboard/tours/update/${id}`}>
          <Gear />
        </Link>
        <Trashcan onClick={() => alert('Not implemented yet')} />
      </td>
    </tr>
  );
};

export default ClassToursTable;