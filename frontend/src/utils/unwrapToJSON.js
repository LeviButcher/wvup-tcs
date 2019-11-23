// eslint-disable-next-line no-undef
const unWrapToJSON = async (fetchPromise: Response) => {
  const res = await fetchPromise;
  return res.json();
};

export default unWrapToJSON;
