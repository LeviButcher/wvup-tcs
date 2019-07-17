import React, { useState, useEffect } from 'react';
import StartToEndDate from '../StartToEndDateForm';
import { Table } from '../../ui';
import ensureResponseCode from '../../utils/ensureResponseCode';
import unwrapToJSON from '../../utils/unwrapToJSON';

const signInData = [
  {
    id: 1,
    email: 'lbutche3@wvup.edu',
    fullName: 'Levi Butcher',
    inTime: new Date(),
    outTime: new Date(),
    tutored: 'true',
    courses: [
      { courseName: 'CS101' },
      { courseName: 'CS121' },
      { courseName: 'CS126' }
    ],
    reasons: [{ reasonName: 'Computer Use' }, { reasonName: 'Chair Use' }]
  }
];

const extraHeaderKey = key => response => response[key];

const getSignInData = (start, end, page = 1) =>
  Promise.resolve({
    status: 200,
    json: () => Promise.resolve(signInData),
    headers: {
      Prev: 'api/lookup/whatever/1',
      Next: 'api/lookup/whatever/1',
      'Page-Count': '50'
    }
  });

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
        .then(setSignIns);
    }
    return () => {
      setSignIns([]);
      setStart('');
      setEnd('');
    };
  }, [startDate, endDate, page]);

  return (
    <div>
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
      {signIns && <SignInsTable signIns={signIns} />}
      {signIns && (
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

const Paging = ({ next, prev, currentPage = 1, baseURL }) => {
  const page = Number(currentPage);
  return (
    <nav>
      {page > 1 && prev && <a href={`${baseURL}${page - 1}`}>Prev</a>}
      <i>{page}</i>
      {next && <a href={`${baseURL}${page + 1}`}>Next</a>}
    </nav>
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

const SignInRow = ({
  signIn: { email, fullName, courses, reasons, inTime, outTime, tutored }
}) => (
  <tr>
    <td>{email}</td>
    <td>{fullName}</td>
    <td>{inTime.toLocaleString()}</td>
    <td>{outTime.toLocaleString()}</td>
    <td>{courses.map(course => course.courseName).join(', ')}</td>
    <td>
      {reasons
        .map(reason => reason.reasonName)
        .concat([tutored ? 'Tutoring' : ''])
        .join(', ')}
    </td>
  </tr>
);

export default SignInLookup;
