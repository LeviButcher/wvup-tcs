import React, { useEffect, useState } from 'react';
import callApi from '../utils/callApi';

const Fetch = ({ url, id, Component, ...props }) => {
  const [object, setObject] = useState();

  useEffect(() => {
    callApi(`${url}${id}`, 'GET', null)
      .then(async res => {
        setObject(await res.json());
      })
      .catch(e => {
        alert(e);
      });
  }, [url, id]);

  return <>{object && <Component data={object} id {...props} />}</>;
};

export default Fetch;
