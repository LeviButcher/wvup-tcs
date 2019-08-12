import React from 'react';
import { Table, Link } from '../ui';
import { Gear, Trashcan } from '../ui/icons';

const SignInsTable = ({ signIns }) => {
  return (
    <Table>
      <thead align="left">
        <tr>
          <th>Email</th>
          <th>Full Name</th>
          <th align="center">In</th>
          <th align="center">Out</th>
          <th align="center">~Total Hours</th>
          <th>Courses</th>
          <th>Reasons</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        {signIns.map(s => (
          <SignInRow signIn={s} key={s.id} />
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

const SignInRow = ({
  signIn: {
    id,
    email,
    fullName,
    courses,
    reasons,
    inTime,
    outTime,
    tutoring,
    type
  }
}) => (
  <tr>
    <td>{email}</td>
    <td>{fullName}</td>
    <td align="center">
      {new Date(inTime).toLocaleDateString('default', dateOptions)}
    </td>
    <td align="center">
      {outTime
        ? new Date(outTime).toLocaleDateString('default', dateOptions)
        : 'Not signed out'}
    </td>
    <td align="center">
      {outTime
        ? new Date(outTime).getHours() - new Date(inTime).getHours()
        : ''}
    </td>
    <td>{courses.map(course => course.shortName).join(', ')}</td>
    <td>
      {reasons
        .map(reason => reason.name)
        .concat([tutoring ? 'Tutoring' : null])
        .concat([type === 1 ? 'Teacher Volunteering' : null])
        .filter(x => x !== null)
        .join(', ')}
    </td>
    <td style={{ display: 'flex', justifyContent: 'space-evenly' }}>
      <Link
        to={
          type === 0
            ? `/dashboard/signins/${id}`
            : `/dashboard/signins/teacher/${id}`
        }
      >
        <Gear />
      </Link>
      <Trashcan onClick={() => alert('Not implemented yet')} />
    </td>
  </tr>
);

export default SignInsTable;
