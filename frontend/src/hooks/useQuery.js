import { useEffect, useState } from 'react';

const useQuery = queryFunc => {
  const [data, setData] = useState();
  const [rerun, setRerun] = useState(false);

  const triggerRerun = () => setRerun(!rerun);

  useEffect(() => {
    queryFunc().then(res => {
      setData(res);
    });
  }, [rerun]);

  return [data, triggerRerun];
};

export default useQuery;
