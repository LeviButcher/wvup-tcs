import React from 'react';
import { Link, navigate } from '@reach/router';
import LoadingContent from '../../components/LoadingContent';
import { Table, Header, Card } from '../../ui';
import { callApi } from '../../utils';
import { Gear, Trashcan } from '../../ui/icons';
import useApiWithHeaders from '../../hooks/useApiWithHeaders';

const deleteUser = id => callApi(`users/${id}`, 'DELETE', null);

const UserManagement = () => {
  const [loading, data, errors] = useApiWithHeaders('users/');

  return (
    <div>
      <a href="users/create">Add User</a>
      <LoadingContent loading={loading} data={data} errors={errors}>
        <Card width="auto">
          <UserTable users={data.body} navigate={navigate} />
        </Card>
      </LoadingContent>
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
