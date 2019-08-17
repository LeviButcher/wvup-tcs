import React from 'react';
import { Router } from '@reach/router';
import { CSVLink } from 'react-csv';
import LoadingContent from '../../components/LoadingContent';
import SemesterForm from '../../components/SemesterForm';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';

const SemesterSignIns = ({ navigate, '*': unMatchedUri }) => {
  const [semesterUri] = unMatchedUri.split('/');
  return (
    <>
      <SemesterForm
        name="Semester Lookup"
        initialValues={{ semester: semesterUri }}
        onSubmit={({ semester }, { setSubmitting }) => {
          navigate(semester);
          setSubmitting(false);
        }}
      />
      <Router primary={false} component={({ children }) => <>{children}</>}>
        <SemesterResults path=":semester" />
      </Router>
    </>
  );
};

const SemesterResults = ({ semester }) => {
  const [loading, data, errors] = useApiWithHeaders(
    `lookups/semester/${semester}`
  );
  return (
    <>
      {loading && <h1>This might be a while...</h1>}
      <LoadingContent data={data} loading={loading} errors={errors}>
        <h1>There were {data.body.length} signins during this semester</h1>
        <h2>
          <CSVLink data={data.body} filename={`semester-${semester}-signins`}>
            Download Now
          </CSVLink>
        </h2>
      </LoadingContent>
    </>
  );
};

export default SemesterSignIns;
