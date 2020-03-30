import React, { useState, useEffect, useCallback } from 'react';
import { Field } from 'formik';
import * as Yup from 'yup';
import ScaleLoader from 'react-spinners/ScaleLoader';
import { Router } from '@reach/router';
import StartToEndDateSchema from '../../schemas/StartToEndDateSchema';
import StartToEndDate from '../../components/StartToEndDateForm';
import { Link, Card, Button, Input } from '../../ui';
import Paging from '../../components/Paging';
import SignInsTable from '../../components/SignInsTable';
import useApi from '../../hooks/useApi';

const SignInLookupSchema = StartToEndDateSchema.shape({
  email: Yup.string()
    .email('Invalid email')
    .matches(/^[A-Z0-9._%+-]+@wvup.edu$/i, 'Must be a wvup email address')
    .trim(),
  crn: Yup.number()
    .positive()
    .typeError('Must be a number')
});

const getSignInUrl = (start, end, crn = '', email = '', page = 1) => {
  if (start == null || end == null) return '';
  const requiredEndpoint = `sessions?start=${start}&end=${end}&page=${page}`;
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

type Props = {
  navigate: any
};

const SignInLookup = ({ navigate }: Props) => {
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
              <Button align="left">Create Sign In</Button>
            </Link>
            <Link to="/dashboard/signins/semester">
              <Button align="left">Download Semesters Signins</Button>
            </Link>
            <Link to="/dashboard/signins/upload">
              <Button align="left">Upload Signins</Button>
            </Link>
          </div>
        </Card>

        <StartToEndDate
          title="Sign In Lookup"
          onSubmit={values => {
            return Promise.resolve(
              navigate(
                `${values.startDate}/${values.endDate}/1/?email=${values.email}&crn=${values.crn}`
              )
            );
          }}
          initialValues={{
            startDate: startDate || '',
            endDate: endDate || '',
            email: email || '',
            crn: crn || ''
          }}
          validationSchema={SignInLookupSchema}
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
          {/* $FlowFixMe */}
          <LookupResults
            path=":startDate/:endDate/:page"
            setFormValues={setFormValues}
          />
        </Router>
      </div>
    </>
  );
};

type LookupResultsProps = {
  startDate: string,
  endDate: string,
  page: number,
  setFormValues: ({}) => any
};

const LookupResults = ({
  startDate,
  endDate,
  page,
  setFormValues
}: LookupResultsProps) => {
  const cachedSetFormValues = useCallback(args => setFormValues(args), [
    setFormValues
  ]);
  const { email, crn } = getQueryParams(window.location.search);
  const endPoint = getSignInUrl(startDate, endDate, crn, email, page);
  const [loading, sessionsPage] = useApi(endPoint);

  useEffect(() => {
    cachedSetFormValues({ startDate, endDate, email, crn });
  }, [startDate, endDate, email, crn, cachedSetFormValues]);

  return (
    <>
      <ScaleLoader sizeUnit="px" size={150} loading={loading} align="center" />
      {!loading && sessionsPage && sessionsPage.data.length < 1 && (
        <h3>No records found for search</h3>
      )}
      {!loading && sessionsPage && sessionsPage.data.length >= 1 && (
        <Card width="auto">
          <Paging
            currentPage={sessionsPage.currentPage}
            totalPages={sessionsPage.totalPages}
            basePath={`/dashboard/signins/${startDate}/${endDate}`}
          />
          <SignInsTable signIns={sessionsPage.data} />
          <Paging
            currentPage={sessionsPage.currentPage}
            totalPages={sessionsPage.totalPages}
            basePath={`/dashboard/signins/${startDate}/${endDate}`}
          />
        </Card>
      )}
    </>
  );
};

export default SignInLookup;
