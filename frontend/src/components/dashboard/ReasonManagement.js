import React from 'react';
import { pipe } from 'ramda';
import { Link } from '@reach/router';
import { Table, Header, Button } from '../../ui';
import useQuery from '../../hooks/useQuery';
import callApi from '../../utils/callApi';

const getReasons = () =>
  callApi(`${process.env.REACT_APP_BACKEND}reasons/`, 'GET', null);

const unWrapFetch = async fetchPromise => {
  const res = await fetchPromise;
  return res.json();
};

const queryReasons = pipe(
  getReasons,
  unWrapFetch
);

const ReasonManagement = () => {
  const [reasons] = useQuery(queryReasons);
  console.log(reasons);
  return (
    <div>
      <a href="reason/create">Add Reason</a>
      {reasons && <ReasonTable reasons={reasons} />}
    </div>
  );
};

const ReasonTable = ({ reasons }) => (
  <Table>
    <caption>
      <Header>Reasons For Visiting</Header>
    </caption>
    <thead align="left">
      <tr>
        <th>Name</th>
        <th>Inactive</th>
        <th>Actions</th>
      </tr>
    </thead>
    <tbody>
      {reasons
        .sort(ele => ele.deleted)
        .map(reason => (
          <tr key={reason.name}>
            <td>{reason.name}</td>
            <td>
              <input type="checkbox" checked={reason.deleted} readOnly />
            </td>
            <td>
              <Link to={`update/${reason.id}`}>
                <Button
                  display="inline-block"
                  intent="secondary"
                  style={{ marginRight: '1rem' }}
                >
                  Update
                </Button>
              </Link>
            </td>
          </tr>
        ))}
    </tbody>
  </Table>
);

export default ReasonManagement;
