const ensureResponseCode = (code: number) => async (fetchPromise: any) => {
  const res = await fetchPromise;
  if (res.status !== code) {
    const errors = await res.json();
    throw errors;
  }
  return fetchPromise;
};

export default ensureResponseCode;
