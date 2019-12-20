import React from 'react';
import { Link } from '@reach/router';
import { Table, Header, Card } from '../../ui';
import useApi from '../../hooks/useApi';
import { Gear } from '../../ui/icons';

const ReasonManagement = () => {
  const [loading, reasons] = useApi('reasons/');

  return (
    <div>
      <Link to="create">Add Reason</Link>
      {loading && <div>Loading...</div>}
      {!loading && reasons && (
        <Card width="auto">
          <ReasonTable reasons={reasons} />
        </Card>
      )}
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
