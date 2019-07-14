const ensureResponseCode = code => async fetchPromise => {
  const res = await fetchPromise;
  if (res.status !== code) {
    let errors = '';
    try {
      errors = await res.clone().json();
    } catch (e) {
      errors = await res.clone().text();
    }
    throw Error(errors);
  }
  return fetchPromise;
};

export default ensureResponseCode;
