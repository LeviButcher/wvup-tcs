import React, { useState, useEffect } from 'react';
import StartToEndDate from '../StartToEndDateForm';
import { Table, Paging, Link, Card, Button } from '../../ui';
import { Gear, Trashcan } from '../../ui/icons';
import { callApi, ensureResponseCode, unwrapToJSON } from '../../utils';

const extraHeaderKey = key => response => response[key];

const take = 20;
const getSignInData = (start, end, page = 1) =>
  callApi(
    `lookups/?start=${start}&end=${end}&skip=${page * take -
      take}&take=${take}`,
    'GET',
    null
  );

const SignInLookup = ({ startDate, endDate, page }) => {
  const [signIns, setSignIns] = useState([]);
  const [start, setStart] = useState(startDate);
  const [end, setEnd] = useState(endDate);
  const [next, setNext] = useState(false);
  const [prev, setPrev] = useState(false);

  useEffect(() => {
    if (page !== undefined) {
      getSignInData(start, end, page)
        .then(ensureResponseCode(200))
        .then(response => {
          setNext(extraHeaderKey('Next', response) !== undefined);
          setPrev(extraHeaderKey('Prev', response) !== undefined);
          return response;
        })
        .then(unwrapToJSON)
        .then(setSignIns)
        .then(() => {
          setStart(startDate);
          setEnd(endDate);
        });
    }
    return () => {
      setSignIns([]);
      setStart('');
      setEnd('');
    };
  }, [startDate, endDate, page]);

  return (
    <div>
      <Card>
        <h3>Additional Actions</h3>
        <Link to="/dashboard/signins/create">
          <Button align="left">Create Sign In</Button>
        </Link>
      </Card>

      <StartToEndDate
        name="Sign In Lookup"
        onSubmit={(values, { setSubmitting, setStatus }) => {
          getSignInData(values.startDate, values.endDate)
            .then(ensureResponseCode(200))
            .then(response => {
              setNext(extraHeaderKey('Next', response) !== 'undefined');
              setPrev(extraHeaderKey('Prev', response) !== 'undefined');
              return response;
            })
            .then(unwrapToJSON)
            .then(setSignIns)
            .then(() => {
              setStart(values.startDate);
              setEnd(values.endDate);
            })
            .catch(e => setStatus({ msg: e.message }))
            .finally(() => setSubmitting(false));
        }}
        startDate={start}
        endDate={end}
        submitText="Run Lookup"
      />
      {signIns && start && end && (
        <Paging
          currentPage={page}
          next={next}
          prev={prev}
          baseURL={`/dashboard/signins/${start}/${end}/`}
        />
      )}
      {signIns && start && end && <SignInsTable signIns={signIns} />}
      {signIns && start && end && (
        <Paging
          currentPage={page}
          next={next}
          prev={prev}
          baseURL={`/dashboard/signins/${start}/${end}/`}
        />
      )}
    </div>
  );
};

const SignInsTable = ({ signIns }) => {
  return (
    <Table>
      <thead align="left">
        <tr>
          <th>Email</th>
          <th>Full Name</th>
          <th>InTime</th>
          <th>OutTime</th>
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
  weekday: 'long',
  year: 'numeric',
  month: 'long',
  day: 'numeric',
  hourCycle: 'h12',
  hour: '2-digit',
  minute: '2-digit'
};

const SignInRow = ({
  signIn: { id, email, fullName, courses, reasons, inTime, outTime, tutored }
}) => (
  <tr>
    <td>{email}</td>
    <td>{fullName}</td>
    <td>{new Date(inTime).toLocaleDateString('default', dateOptions)}</td>
    <td>{new Date(outTime).toLocaleDateString('default', dateOptions)}</td>
    <td>{courses.map(course => course.shortName).join(', ')}</td>
    <td>
      {reasons
        .map(reason => reason.name)
        .concat([tutored ? 'Tutoring' : null])
        .filter(x => x !== null)
        .join(', ')}
    </td>
    <td style={{ display: 'flex', justifyContent: 'space-evenly' }}>
      <Link to={`/dashboard/signins/${id}`}>
        <Gear />
      </Link>
      <Trashcan onClick={() => alert('Not implemented yet')} />
    </td>
  </tr>
);

export default SignInLookup;
