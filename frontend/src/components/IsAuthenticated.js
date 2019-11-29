import React, { useEffect, useState } from 'react';
import type { Node } from 'react';
import { navigate } from '@reach/router';
import callApi from '../utils/callApi';

const checkAuth = () => callApi(`users/IsAuthenticate`, 'GET', null);

type Props = {
  redirectRoute: string,
  children: Node
};

// Redirect to route is not signed in, else render component
// Need to add loading state
const IsAuthenticated = ({ redirectRoute, children }: Props) => {
  const [isAuth, setAuth] = useState(false);

  useEffect(() => {
    checkAuth()
      .then(async res => {
        if (res.status !== 200 || (await res.json()).authenticated !== true) {
          navigate(redirectRoute);
        }
        setAuth(true);
      })
      .catch(() => {
        navigate(redirectRoute);
      });
  }, [redirectRoute, children]);

  return <div>{isAuth && children}</div>;
};

export default IsAuthenticated;
