import { useEffect, useState, useCallback } from 'react';

const useQuery = (queryFunc: Function) => {
  const queryFuncCallback = useCallback(() => queryFunc(), [queryFunc]);
  const [data, setData] = useState();
  const [rerun, setRerun] = useState(false);

  const triggerRerun = () => setRerun(!rerun);

  useEffect(() => {
    queryFuncCallback().then(res => {
      setData(res);
    });
  }, [rerun, queryFuncCallback]);

  return [data, triggerRerun];
};

export default useQuery;
