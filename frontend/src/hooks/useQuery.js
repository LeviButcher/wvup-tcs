import { useEffect, useState } from 'react';

const useQuery = queryFunc => {
  const [data, setData] = useState();

  useEffect(() => {
    queryFunc().then(res => {
      setData(res);
    });
  }, []);

  return [data];
};

export default useQuery;
