import React from 'react';
import { pipe } from 'ramda';
import { Link } from '@reach/router';
import { Table, Header, Button } from '../../ui';
import callApi from '../../utils/callApi';
import useQuery from '../../hooks/useQuery';

const getUsers = callApi(`${process.env.REACT_APP_BACKEND}users/`, 'GET');

const deleteUser = id =>
  callApi(`${process.env.REACT_APP_BACKEND}users/${id}`, 'DELETE', null);

const unWrapFetch = async fetchFunc => {
  const res = await fetchFunc(null);
  return res.json();
};

const queryUsers = pipe(
  getUsers,
  unWrapFetch
);

const UserManagement = () => {
  const [users, reload] = useQuery(queryUsers);

  return (
    <div>
      <a href="users/create">Add User</a>
      {users && <UserTable users={users} afterDelete={reload} />}
    </div>
  );
};

const UserTable = ({ users, afterDelete }) => (
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
            <Link to={`update/${user.id}`}>
              <Button
                display="inline-block"
                intent="secondary"
                style={{ margin: '0 1rem' }}
              >
                Update
              </Button>
            </Link>
            <Button
              display="inline-block"
              intent="danger"
              style={{ margin: '0 1rem' }}
              onClick={() => {
                if (user.username === localStorage.getItem('username')) {
                  alert("You can't delete yourself silly");
                } else {
                  const goDelete = window.confirm(
                    `Are you sure you want to delete ${user.username}`
                  );
                  if (goDelete) deleteUser(user.id).then(() => afterDelete());
                }
              }}
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
