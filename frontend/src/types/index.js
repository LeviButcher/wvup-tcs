// Describe Universal Types of the application here
// These types should closely mirror the classes on the backend
const personTypeValues = {
  student: 0,
  teacher: 1
};

// eslint-disable-next-line import/prefer-default-export
export { personTypeValues };

// eslint-disable-next-line no-undef
export type PersonType = 0 | 1;

export type Course = {
  crn: string,
  shortName: string
};

export type Semester = {
  code: number,
  name: string
};

export type Student = {
  email: string,
  id: string,
  firstName: string,
  lastName: string,
  schedule: Array<Course>
};

export type Reason = {
  id?: string,
  name: string,
  deleted: boolean
};

export type Teacher = {
  id: string,
  email: string
};

export type ClassTour = {
  id?: string,
  name: string,
  dayVisited: string,
  numberOfStudents: number
};

export type User = {
  token?: string,
  username: string,
  firstName?: string,
  lastName?: string,
  password?: string,
  id?: string
};

export type SignIn = {
  id: string,
  email: string,
  fullName: string,
  classes: [Course],
  reasons: [Reason],
  inTime: string,
  outTime: string,
  tutoring: string,
  type: string
};
