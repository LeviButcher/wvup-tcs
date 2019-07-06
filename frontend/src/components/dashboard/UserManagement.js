import React from 'react';
import { pipe } from 'ramda';
import { Link } from '@reach/router';
import { Table, Header, Button } from '../../ui';
import callApi from '../../utils/callApi';
import useQuery from '../../hooks/useQuery';

const getUsers = callApi(`${process.env.REACT_APP_BACKEND}users/`, 'GET');

const unWrapFetch = async fetchFunc => {
  const res = await fetchFunc(null);
  return res.json();
};

const queryUsers = pipe(
  getUsers,
  unWrapFetch
);

const UserManagement = () => {
  const [users] = useQuery(queryUsers);

  return (
    <div>
      <a href="users/create">Add User</a>
      {users && <UserTable users={users} />}
    </div>
  );
};

const UserTable = ({ users }) => (
  <Table>
    <caption>
      <Header>Users In System</Header>
    </caption>
    <thead align="left">
      <tr>
        <th>Username</th>
        <th>FullName</th>
        <th>Actions</th>
      </tr>
    </thead>
    <tbody>
      {users.map(user => (
        <tr>
          <td>{user.username}</td>
          <td>{`${user.firstName} ${user.lastName}`}</td>
          <td>
            <Button
              display="inline-block"
              intent="secondary"
              style={{ margin: '0 1rem' }}
            >
              <Link to={`update/${user.id}`}>Update</Link>
            </Button>
            <Button
              display="inline-block"
              intent="danger"
              style={{ margin: '0 1rem' }}
              onClick={() => alert('Yell at levi todo')}
            >
              Delete
            </Button>
          </td>
        </tr>
      ))}
    </tbody>
  </Table>
);

export default UserManagement;
