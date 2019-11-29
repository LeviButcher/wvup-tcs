import React from 'react';
import { Router } from '@reach/router';
import { CSVLink } from 'react-csv';
import LoadingContent from '../../components/LoadingContent';
import SemesterForm from '../../components/SemesterForm';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';

type Props = {
  navigate: any,
  '*': string
};

const SemesterSignIns = ({ navigate, '*': unMatchedUri }: Props) => {
  const [semesterUri] = unMatchedUri.split('/');
  return (
    <>
      <SemesterForm
        title="Semester Lookup"
        initialValues={{ semester: semesterUri }}
        onSubmit={({ semester }) => {
          return Promise.resolve(navigate(semester));
        }}
      />
      <Router primary={false} component={({ children }) => <>{children}</>}>
        {/* $FlowFixMe */}
        <SemesterResults path=":semester" />
      </Router>
    </>
  );
};

type SemesterResultsProps = {
  semester: string
};

const SemesterResults = ({ semester }: SemesterResultsProps) => {
  const [loading, data, errors] = useApiWithHeaders(
    `lookups/semester/${semester}`
  );
  return (
    <>
      {loading && <h1>This might be a while...</h1>}
      <LoadingContent data={data} loading={loading} errors={errors}>
        <h1>There were {data.body.length} signins during this semester</h1>
        <h2>
          <CSVLink
            data={data.body}
            filename={`semester-${semester}-signins.csv`}
          >
            Download Now
          </CSVLink>
        </h2>
      </LoadingContent>
    </>
  );
};

export default SemesterSignIns;
