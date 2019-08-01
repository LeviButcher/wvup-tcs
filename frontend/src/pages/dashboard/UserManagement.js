import React from 'react';
import { pipe } from 'ramda';
import { Link } from '@reach/router';
import { Table, Header } from '../../ui';
import { callApi, ensureResponseCode, unwrapToJSON } from '../../utils';
import { Gear, Trashcan } from '../../ui/icons';
import useQuery from '../../hooks/useQuery';

const getUsers = () => callApi(`users/`, 'GET', null);
const deleteUser = id => callApi(`users/${id}`, 'DELETE', null);

const queryUsers = pipe(
  getUsers,
  ensureResponseCode(200),
  unwrapToJSON
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
        <tr key={user.id}>
          <td>{user.username}</td>
          <td>{`${user.firstName} ${user.lastName}`}</td>
          <td>
            <Link to={`update/${user.id}`}>
              <Gear />
            </Link>
            <Trashcan
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
            />
          </td>
        </tr>
      ))}
    </tbody>
  </Table>
);

export default UserManagement;
