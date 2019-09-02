import React, { useState, useEffect } from 'react';
import ScaleLoader from 'react-spinners/ScaleLoader';
import { Router } from '@reach/router';
import StartToEndDate from '../../components/StartToEndDateForm';
import ClassToursTable from '../../components/ClassToursTable';
import { Paging, Link, Button, Card } from '../../ui';
import StartToEndDateSchema from '../../schemas/StartToEndDateSchema';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';

const take = 20;
const getClassTourUrl = (start, end, page = 1) => {
  if (start == null || end == null) return '';
  const endpoint = `classtours/?start=${start}&end=${end}&skip=${page * take -
    take}&take=${take}`;
  return endpoint;
};

const ClassTourLookup = ({ navigate }) => {
  const [{ startDate, endDate }, setFormValues] = useState({});

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
            <Link to="create">
              <Button intent="primary" align="left">
                Add Class Tour
              </Button>
            </Link>
          </div>
        </Card>

        <StartToEndDate
          name="ClassTour Lookup"
          onSubmit={(values, { setSubmitting }) => {
            navigate(`${values.startDate}/${values.endDate}/1`);
            setSubmitting(false);
          }}
          submitText="Run Lookup"
          initialValues={{
            startDate: startDate || '',
            endDate: endDate || ''
          }}
          isInitialValid={false}
          validationSchema={StartToEndDateSchema}
          enableReinitialize
        ></StartToEndDate>
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
  const endpoint = getClassTourUrl(startDate, endDate, page);
  const [loading, data] = useApiWithHeaders(endpoint);
  useEffect(() => {
    setFormValues({ startDate, endDate });
  }, [startDate, endDate]);

  return (
    <>
      <ScaleLoader sizeUnit="px" size={150} loading={loading} align="center" />
      {!loading && data.body.length < 1 && <h3>No reconds found for search</h3>}
      {!loading && data && data.headers && data.body.length >= 1 && (
        <Card width="1400px">
          <Paging
            currentPage={data.headers['current-page']}
            totalPages={data.headers['total-pages']}
            next={data.headers.next}
            prev={data.headers.prev}
            queries={{}}
            baseURL={`/dashboard/tours/${startDate}/${endDate}/`}
          />

          <ClassToursTable classTours={data.body} />

          <Paging
            currentPage={data.headers['current-page']}
            totalPages={data.headers['total-pages']}
            next={data.headers.next}
            prev={data.headers.prev}
            queries={{}}
            baseURL={`/dashboard/tours/${startDate}/${endDate}/`}
          />
        </Card>
      )}
    </>
  );
};

export default ClassTourLookup;
