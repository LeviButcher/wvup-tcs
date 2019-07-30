import React, { useState, useEffect } from 'react';
import StartToEndDate from '../StartToEndDateForm';
import { Paging, Link, Card, Button } from '../../ui';
import { callApi, ensureResponseCode, unwrapToJSON } from '../../utils';
import SignInsTable from '../SignInsTable';

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

export default SignInLookup;
