import React, { useState } from 'react';
import { navigate } from '@reach/router';
import styled from 'styled-components';
import { Card, Stack, Button, Header } from '../../ui';

const Label = styled.label`
  display: block;
`;

const UploadSignIns = () => {
  const [files, setFiles] = useState([]);
  const [error, setError] = useState();
  const [submitting, setSubmitting] = useState(false);

  return (
    <Card>
      <form
        onSubmit={async e => {
          e.preventDefault();
          setSubmitting(true);
          const formData = new FormData();
          formData.append('csvFile', files[0]);
          const token = localStorage.getItem(
            `${process.env.REACT_APP_TOKEN || ''}`
          );
          const res = await fetch(
            `${process.env.REACT_APP_BACKEND || ''}sessions/upload`,
            {
              method: 'POST',
              headers: {
                Authorization: `Bearer ${token || ''}`
              },
              body: formData
            }
          ).catch(() => {
            return {
              status: 500,
              json: () => Promise.resolve({ message: 'Something went wrong' })
            };
          });

          if (res.status === 200) {
            // eslint-disable-next-line no-alert, no-undef
            alert('Successfully Uploaded CSV Records!');
            navigate('.');
          } else {
            setError(await res.json());
          }

          setSubmitting(false);
        }}
      >
        <Stack>
          <Header>Sign In Upload</Header>
          <p>
            Sign In Upload takes a CSV File with Sign In Records and saves it to
            TCS's Database
          </p>
          <p>
            Your CSV File must contain the following headers: Email, InTime,
            OutTime, CRNs, Reasons, SemesterCode, Tutoring
          </p>
          <p>
            If a Email, CRN, Reason, or Semester Code do not exist in the
            Database, then Uploading will fail
          </p>
          <p>
            Please read here for{' '}
            <a href="https://github.com/LeviButcher/wvup-tcs/wiki/User-Documentation">
              more information about Sign In Upload
            </a>
          </p>
          <Label htmlFor="file">CSV File</Label>
          <input
            type="file"
            id="file"
            required
            files={files}
            accept="text/csv"
            onChange={e => {
              setFiles(e.target.files);
            }}
          />
          <div style={{ color: 'red' }}>{error && error.message}</div>
          <Button
            type="submit"
            fullWidth
            disabled={files.length === 0 || submitting}
          >
            {submitting ? 'Submitting...' : 'Submit'}
          </Button>
        </Stack>
      </form>
    </Card>
  );
};

export default UploadSignIns;
