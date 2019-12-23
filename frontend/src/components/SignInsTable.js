import React from 'react';
import { Table, Link } from '../ui';
import { Gear, Trashcan } from '../ui/icons';
import { callApi } from '../utils';
import type { SignInDisplay } from '../types';

function hourDifferenceInTime(date, date2) {
  const { bigDate, smallDate } =
    date > date2
      ? { bigDate: date, smallDate: date2 }
      : { bigDate: date2, smallDate: date };
  const diff = (bigDate.getTime() - smallDate.getTime()) / 1000;
  return (diff / 60 / 60).toFixed(2);
}

type Props = {
  signIns: Array<SignInDisplay>
};

const removeSession = id => callApi(`sessions/${id}`, 'DELETE', null);

const SignInsTable = ({ signIns }: Props) => {
  return (
    <Table>
      <thead align="left">
        <tr>
          <th>Email (@wvup.edu)</th>
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
    person: { email, personType, fullName },
    selectedClasses,
    selectedReasons,
    inTime,
    outTime,
    tutoring
  }
}) => {
  return (
    <tr>
      <td style={{ maxWidth: '150px' }}>{email.split('@')[0]}</td>
      <td>{`${fullName}`}</td>
      <td align="center">
        {new Date(inTime).toLocaleDateString(undefined, dateOptions)}
      </td>
      <td align="center">
        {outTime
          ? new Date(outTime).toLocaleDateString(undefined, dateOptions)
          : 'Not signed out'}
      </td>
      <td align="center">
        {outTime
          ? hourDifferenceInTime(new Date(inTime), new Date(outTime))
          : ''}
      </td>
      <td>{selectedClasses.map(course => course.shortName).join(', ')}</td>
      <td>
        {selectedReasons
          .map(reason => reason.name)
          .concat([tutoring ? 'Tutoring' : null])
          .concat([personType === 1 ? 'Teacher Volunteering' : null])
          .filter(x => x !== null)
          .join(', ')}
      </td>
      <td style={{ display: 'flex', justifyContent: 'space-evenly' }}>
        <Link to={`/dashboard/signins/${id}`}>
          <Gear />
        </Link>
        <Link to={`${window.location.pathname}${window.location.search}`}>
          <Trashcan onClick={() => removeSession(id)} />
        </Link>
      </td>
    </tr>
  );
};

export default SignInsTable;
