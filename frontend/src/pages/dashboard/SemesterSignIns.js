import React from 'react';
import { Router } from '@reach/router';
import { CSVLink } from 'react-csv';
import SemesterForm from '../../components/SemesterForm';
import useApi from '../../hooks/useApi';

type Props = {
  navigate: any,
  '*': string
};

const headers = [
  { label: 'Session Id', key: 'id' },
  { label: 'Full Name', key: 'person.fullName' },
  { label: 'Email', key: 'person.email' },
  { label: 'In Time', key: 'inTime' },
  { label: 'Out Time', key: 'outTime' },
  { label: 'Tutoring', key: 'tutoring' },
  { label: 'Classes', key: 'selectedClasses' },
  { label: 'Reasons', key: 'selectedReasons' },
  { label: 'Semester', key: 'semester.name' },
  { label: 'Semester Code', key: 'semester.code' }
];

const formatToCorrectTableForm = sessions => {
  return sessions.map(x => ({
    ...x,
    selectedClasses: x.selectedClasses.map(c => c.name),
    selectedReasons: x.selectedReasons.map(r => r.name)
  }));
};

const SemesterSignIns = ({ navigate, '*': unMatchedUri }: Props) => {
  const [semesterUri] = unMatchedUri ? unMatchedUri.split('/') : [''];
  return (
    <>
      <SemesterForm
        title="Semester Lookup"
        initialValues={{ semesterCode: semesterUri }}
        onSubmit={({ semesterCode }) => {
          return Promise.resolve(navigate(semesterCode));
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
  const [loading, semesterSessions] = useApi(`sessions/semester/${semester}`);
  return (
    <>
      {loading && <h1>This might be a while...</h1>}
      {!loading && semesterSessions && (
        <>
          <h1>
            There were {semesterSessions.length} signins during this semester
          </h1>
          <h2>
            <CSVLink
              headers={headers}
              data={formatToCorrectTableForm(semesterSessions)}
              filename={`semester-${semester}-signins.csv`}
            >
              Download Now
            </CSVLink>
          </h2>
        </>
      )}
    </>
  );
};

export default SemesterSignIns;
