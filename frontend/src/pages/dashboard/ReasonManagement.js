import React from 'react';
import { Link } from '@reach/router';
import { Table, Header } from '../../ui';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';
import { Gear } from '../../ui/icons';
import LoadingContent from '../../components/LoadingContent';

const ReasonManagement = () => {
  const [loading, data, errors] = useApiWithHeaders('reasons/');

  return (
    <div>
      <Link to="create">Add Reason</Link>
      <LoadingContent loading={loading} data={data} errors={errors}>
        <ReasonTable reasons={data.body} />
      </LoadingContent>
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
