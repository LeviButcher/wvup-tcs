import React, { useState, useEffect } from 'react';
import StartToEndDate from '../StartToEndDateForm';
import { Table, Paging, Link, Card, Button } from '../../ui';
import { Gear, Trashcan } from '../../ui/icons';
import { callApi, ensureResponseCode, unwrapToJSON } from '../../utils';

const extraHeaderKey = key => response => response.headers.get(key);
const getNextHeader = extraHeaderKey('Next');
const getPrevHeader = extraHeaderKey('Prev');
const getCurrentPageHeader = extraHeaderKey('Current-Page');
const getTotalPagesHeader = extraHeaderKey('Total-Pages');

const take = 20;
const getSignInData = (start, end, page = 1) =>
  callApi(
    `lookups/?start=${start}&end=${end}&skip=${page * take -
      take}&take=${take}`,
    'GET',
    null
  );
const defaultPagingState = {
  next: false,
  prev: false,
  page: 1,
  totalPages: 1
};
const SignInLookup = ({ startDate, endDate, page }) => {
  const [signIns, setSignIns] = useState([]);
  const [start, setStart] = useState(startDate);
  const [end, setEnd] = useState(endDate);
  const [paging, setPaging] = useState({ ...defaultPagingState, page });

  useEffect(() => {
    if (page !== undefined) {
      getSignInData(start, end, page)
        .then(ensureResponseCode(200))
        .then(response => {
          setPaging({
            next: getNextHeader(response) !== null,
            prev: getPrevHeader(response) !== null,
            page: getCurrentPageHeader(response),
            totalPages: getTotalPagesHeader(response)
          });
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
      setPaging(defaultPagingState);
    };
  }, [startDate, endDate, page]);

  return (
    <>
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
                setPaging({
                  next: getNextHeader(response) !== null,
                  prev: getPrevHeader(response) !== null,
                  page: getCurrentPageHeader(response),
                  totalPages: getTotalPagesHeader(response)
                });
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
      </div>
      {signIns && start && end && (
        <Card width="1400px">
          <Paging
            currentPage={paging.page}
            totalPages={paging.totalPages}
            next={paging.next}
            prev={paging.prev}
            baseURL={`/dashboard/signins/${start}/${end}/`}
          />
          <SignInsTable signIns={signIns} />
          <Paging
            currentPage={paging.page}
            totalPages={paging.totalPages}
            next={paging.next}
            prev={paging.prev}
            baseURL={`/dashboard/signins/${start}/${end}/`}
          />
        </Card>
      )}
    </>
  );
};

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
  signIn: { id, email, fullName, courses, reasons, inTime, outTime, tutoring }
}) => (
  <tr>
    <td>{email}</td>
    <td>{fullName}</td>
    <td align="center">
      {new Date(inTime).toLocaleDateString('default', dateOptions)}
    </td>
    <td align="center">
      {new Date(outTime).toLocaleDateString('default', dateOptions)}
    </td>
    <td align="center">
      {new Date(outTime).getHours() - new Date(inTime).getHours()}
    </td>
    <td>{courses.map(course => course.shortName).join(', ')}</td>
    <td>
      {reasons
        .map(reason => reason.name)
        .concat([tutoring ? 'Tutoring' : null])
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
