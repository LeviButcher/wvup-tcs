const unWrapToJSON = async fetchPromise => {
  const res = await fetchPromise;
  return res.json();
};

export default unWrapToJSON;
