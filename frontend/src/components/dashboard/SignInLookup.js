import React, { useState, useEffect } from 'react';
import { GoTrashcan, GoGear } from 'react-icons/go';
import StartToEndDate from '../StartToEndDateForm';
import { Table, Paging, Link } from '../../ui';
import ensureResponseCode from '../../utils/ensureResponseCode';
import unwrapToJSON from '../../utils/unwrapToJSON';
import callApi from '../../utils/callApi';

const extraHeaderKey = key => response => response[key];

const take = 20;
const getSignInData = (start, end, page = 1) =>
  callApi(
    `${
      process.env.REACT_APP_BACKEND
    }lookups/?start=${start}&end=${end}&skip=${page * take -
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
      <div>
        <h3>Additional Actions</h3>
        <Link to="/dashboard/signins/create">Create Sign In</Link>
      </div>

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
        .concat([tutored ? 'Tutoring' : ''])
        .join(', ')}
    </td>
    <td>
      <Link to={`/dashboard/signins/${id}`}>
        <GoGear />
      </Link>
      |
      <GoTrashcan onClick={() => alert('Not implemented yet')} />
    </td>
  </tr>
);

export default SignInLookup;
