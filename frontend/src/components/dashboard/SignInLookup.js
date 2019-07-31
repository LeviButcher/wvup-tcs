import React, { useState, useEffect } from 'react';
import { Field } from 'formik';
import * as Yup from 'yup';
import StartToEndDateSchema from '../../schemas/StartToEndDateSchema';
import StartToEndDate from '../StartToEndDateForm';
import { Paging, Link, Card, Button, Input } from '../../ui';
import { callApi, ensureResponseCode, unwrapToJSON } from '../../utils';
import SignInsTable from '../SignInsTable';

const SignInLookupSchema = StartToEndDateSchema.shape({
  email: Yup.string()
    .email('Invalid email')
    .matches(/^[A-Z0-9._%+-]+@wvup.edu$/i, 'Must be a wvup email address')
    .trim(),
  crn: Yup.number()
    .positive()
    .typeError('Must be a number')
});

const extraHeaderKey = key => response => response.headers.get(key);
const getNextHeader = extraHeaderKey('Next');
const getPrevHeader = extraHeaderKey('Prev');
const getCurrentPageHeader = extraHeaderKey('Current-Page');
const getTotalPagesHeader = extraHeaderKey('Total-Pages');

const take = 20;
const getSignInData = (start, end, crn = '', email = '', page = 1) => {
  const requiredEndpoint = `lookups/?start=${start}&end=${end}&skip=${page *
    take -
    take}&take=${take}`;
  const crnQuery = crn !== '' ? `&crn=${crn}` : '';
  const emailQuery = email !== '' ? `&email=${email}` : '';
  const endPoint = requiredEndpoint.concat('', crnQuery).concat('', emailQuery);
  return callApi(endPoint, 'GET', null);
};

const defaultPagingState = {
  next: false,
  prev: false,
  page: 1,
  totalPages: 1
};

const useQueryParams = () => {
  const [params, setParams] = useState({});
  useEffect(() => {
    const queries = new URLSearchParams(window.location.search);
    const temp = {};
    queries.forEach((value, name) => {
      temp[name] = value;
    });
    setParams(temp);
  }, []);
  return [params];
};

const SignInLookup = ({ startDate = '', endDate = '', page, navigate }) => {
  const [signIns, setSignIns] = useState([]);
  const [start, setStart] = useState(startDate);
  const [end, setEnd] = useState(endDate);
  const [paging, setPaging] = useState({ ...defaultPagingState, page });
  const [{ email = '', crn = '' }] = useQueryParams();

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
            getSignInData(
              values.startDate,
              values.endDate,
              values.crn,
              values.email
            )
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
                navigate(`&email=${email}&crn=${crn}`);
              })
              .catch(e => setStatus({ msg: e.message }))
              .finally(() => setSubmitting(false));
          }}
          submitText="Run Lookup"
          initialValues={{
            startDate: start,
            endDate: end,
            email,
            crn
          }}
          isInitialValid={e => {
            try {
              return SignInLookupSchema.validateSync(e.initialValues);
            } catch {
              return false;
            }
          }}
          validationSchema={SignInLookupSchema}
          enableReinitialize
        >
          <Field
            id="email"
            type="text"
            name="email"
            component={Input}
            label="*Email"
          />
          <Field
            id="crn"
            type="text"
            name="crn"
            component={Input}
            label="*CRN"
          />
        </StartToEndDate>
      </div>
      {signIns && start && end && (
        <Card width="1400px">
          <Paging
            currentPage={paging.page}
            totalPages={paging.totalPages}
            next={paging.next}
            prev={paging.prev}
            queries={{ email, crn }}
            baseURL={`/dashboard/signins/${start}/${end}/`}
          />
          <SignInsTable signIns={signIns} />
          <Paging
            currentPage={paging.page}
            totalPages={paging.totalPages}
            next={paging.next}
            prev={paging.prev}
            queries={{ email, crn }}
            baseURL={`/dashboard/signins/${start}/${end}/`}
          />
        </Card>
      )}
    </>
  );
};

export default SignInLookup;
