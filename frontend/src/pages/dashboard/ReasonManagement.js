import React from 'react';
import { pipe } from 'ramda';
import { Link } from '@reach/router';
import { Table, Header } from '../../ui';
import useQuery from '../../hooks/useQuery';
import { Gear } from '../../ui/icons';
import { callApi, unwrapToJSON, ensureResponseCode } from '../../utils';

const getReasons = () => callApi(`reasons/`, 'GET', null);

const queryReasons = pipe(
  getReasons,
  ensureResponseCode(200),
  unwrapToJSON
);

const ReasonManagement = () => {
  const [reasons] = useQuery(queryReasons);
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
                <Gear />
              </Link>
            </td>
          </tr>
        ))}
    </tbody>
  </Table>
);

export default ReasonManagement;
