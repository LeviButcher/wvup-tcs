const isWVUPEmail = (email: string) =>
  email.match(/^[A-Z0-9._%+-]+@wvup.edu$/i);

export default isWVUPEmail;
