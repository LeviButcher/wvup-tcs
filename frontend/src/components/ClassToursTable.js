import React from 'react';
import { Table, Link } from '../ui';
import { Gear, Trashcan } from '../ui/icons';
import type { ClassTour } from '../types';
import { apiFetch } from '../utils/fetchLight';

type Props = {
  classTours: Array<ClassTour>
};

const ClassToursTable = ({ classTours }: Props) => {
  return (
    <Table>
      <thead align="left">
        <tr>
          <th align="center">Name</th>
          <th align="center">Day Visited</th>
          <th align="center">Number of Students</th>
          <th align="center">Actions</th>
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

type ClassTourRowProps = {
  classTour: ClassTour
};

const ClassTourRow = ({
  classTour: { id, name, dayVisited, numberOfStudents }
}: ClassTourRowProps) => {
  return (
    <tr>
      <td>{name}</td>
      <td align="center">
        {new Date(dayVisited).toLocaleDateString(undefined, dateOptions)}
      </td>
      <td align="center">{numberOfStudents}</td>
      <td style={{ display: 'flex', justifyContent: 'space-evenly' }}>
        <Link to={`/dashboard/tours/update/${id || ''}`}>
          <Gear />
        </Link>
        <Trashcan
          data-testid="delete-classtour"
          onClick={async () => {
            if (
              window.confirm(
                `Are you sure you want to delete ${name} with ${numberOfStudents} students visited?`
              )
            ) {
              await apiFetch(
                `classtours/${id || ''}`,
                'DELETE',
                null
              ).then(() => window.location.reload());
            }
          }}
        />
      </td>
    </tr>
  );
};

export default ClassToursTable;
