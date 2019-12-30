import React from 'react';
import { Link, navigate } from '@reach/router';
import { Table, Header, Card } from '../../ui';
import { apiFetch } from '../../utils/fetchLight';
import { Gear, Trashcan } from '../../ui/icons';
import useApi from '../../hooks/useApi';

const deleteUser = id => apiFetch(`users/${id}`, 'DELETE', null);

const UserManagement = () => {
  const [loading, data] = useApi('users/');

  return (
    <div>
      <Link to="create">Add User</Link>
      {loading && <div>Loading...</div>}
      {!loading && data && (
        <Card width="auto">
          <UserTable users={data} navigate={navigate} />
        </Card>
      )}
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
                  if (goDelete)
                    deleteUser(user.id).then(() => window.location.reload());
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
