import React, { useState, useEffect } from 'react';
import { Field } from 'formik';
import * as Yup from 'yup';
import ScaleLoader from 'react-spinners/ScaleLoader';
import { Router } from '@reach/router';
import StartToEndDateSchema from '../../schemas/StartToEndDateSchema';
import StartToEndDate from '../../components/StartToEndDateForm';
import { Paging, Link, Card, Button, Input } from '../../ui';
import SignInsTable from '../../components/SignInsTable';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';

const SignInLookupSchema = StartToEndDateSchema.shape({
  email: Yup.string()
    .email('Invalid email')
    .matches(/^[A-Z0-9._%+-]+@wvup.edu$/i, 'Must be a wvup email address')
    .trim(),
  crn: Yup.number()
    .positive()
    .typeError('Must be a number')
});

const take = 20;
const getSignInUrl = (start, end, crn = '', email = '', page = 1) => {
  if (start == null || end == null) return '';
  const requiredEndpoint = `lookups/?start=${start}&end=${end}&skip=${page *
    take -
    take}&take=${take}`;
  const crnQuery = crn !== '' ? `&crn=${crn}` : '';
  const emailQuery = email !== '' ? `&email=${email}` : '';
  const endPoint = requiredEndpoint.concat('', crnQuery).concat('', emailQuery);
  return endPoint;
};

const getQueryParams = url => {
  const queries = new URLSearchParams(url);
  const params = {};
  queries.forEach((value, name) => {
    params[name] = value;
  });
  return params;
};

const SignInLookup = ({ navigate }) => {
  const [{ startDate, endDate, email, crn }, setFormValues] = useState({});

  return (
    <>
      <div>
        <Card>
          <h3>Additional Actions</h3>
          <div
            style={{
              display: 'grid',
              justifyContent: 'space-between',
              gridTemplateColumns: 'auto auto',
              gridGap: '10px'
            }}
          >
            <Link to="/dashboard/signins/create">
              <Button align="left">Create Student Sign In</Button>
            </Link>
            <Link to="/dashboard/signins/teacher/create">
              <Button align="left">Create Teacher Sign In</Button>
            </Link>
            <Link to="/dashboard/signins/semester">
              <Button align="left">Download Semesters Signins</Button>
            </Link>
          </div>
        </Card>

        <StartToEndDate
          name="Sign In Lookup"
          onSubmit={(values, { setSubmitting }) => {
            navigate(
              `${values.startDate}/${values.endDate}/1/?email=${values.email}&crn=${values.crn}`
            );
            setSubmitting(false);
          }}
          submitText="Run Lookup"
          initialValues={{
            startDate: startDate || '',
            endDate: endDate || '',
            email: email || '',
            crn: crn || ''
          }}
          isInitialValid={false}
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
        <Router primary={false}>
          <LookupResults
            path=":startDate/:endDate/:page"
            setFormValues={setFormValues}
          />
        </Router>
      </div>
    </>
  );
};

const LookupResults = ({ startDate, endDate, page, setFormValues }) => {
  const { email, crn } = getQueryParams(window.location.search);
  const endPoint = getSignInUrl(startDate, endDate, crn, email, page);
  const [loading, data] = useApiWithHeaders(endPoint);
  useEffect(() => {
    setFormValues({ startDate, endDate, email, crn });
  }, [startDate, endDate, email, crn]);
  console.log(data);
  return (
    <>
      <ScaleLoader sizeUnit="px" size={150} loading={loading} align="center" />
      {!loading && data.body.length < 1 && <h3>No records found for search</h3>}
      {!loading && data && data.headers && data.body.length >= 1 && (
        <Card width="1400px">
          <Paging
            currentPage={data.headers['current-page']}
            totalPages={data.headers['total-pages']}
            next={data.headers.next}
            prev={data.headers.prev}
            queries={{ email, crn }}
            baseURL={`/dashboard/signins/${startDate}/${endDate}/`}
          />
          <SignInsTable signIns={data.body}  />
          <Paging
            currentPage={data.headers['current-page']}
            totalPages={data.headers['total-pages']}
            next={data.headers.next}
            prev={data.headers.prev}
            queries={{ email, crn }}
            baseURL={`/dashboard/signins/${startDate}/${endDate}/`}
          />
        </Card>
      )}
    </>
  );
};

export default SignInLookup;
