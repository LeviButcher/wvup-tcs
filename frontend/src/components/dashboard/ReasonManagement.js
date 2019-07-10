import React from 'react';
import { pipe } from 'ramda';
import { Link } from '@reach/router';
import { Table, Header, Button } from '../../ui';
import useQuery from '../../hooks/useQuery';

const reasonData = [
  { name: 'Bone Use', active: true },
  { name: 'Lab Time', active: true },
  { name: 'Computer Use', active: true }
];

const getReasons = () => () => Promise.resolve({ json: () => reasonData });

const unWrapFetch = async fetchFunc => {
  const res = await fetchFunc(null);
  return res.json();
};

const queryReasons = pipe(
  getReasons,
  unWrapFetch
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
        <th>Active</th>
        <th>Actions</th>
      </tr>
    </thead>
    <tbody>
      {reasons.map(reason => (
        <tr>
          <td>{reason.name}</td>
          <td>
            <input type="checkbox" checked={reason.active} />
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
            <Button
              display="inline-block"
              intent="danger"
              style={{ marginRight: '1rem' }}
              onClick={() => alert("can't do that yet")}
            >
              Delete
            </Button>
          </td>
        </tr>
      ))}
    </tbody>
  </Table>
);

export default ReasonManagement;
