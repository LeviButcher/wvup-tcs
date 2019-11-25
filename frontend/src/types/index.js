// Describe Universal Types of the application here
// These types should closely mirror the classes on the backend

export type Course = {
  crn: string,
  shortName: string
};

export type Student = {
  studentEmail: string,
  studentID: string,
  semesterId: string,
  firstName: string,
  lastName: string,
  classSchedule: Array<Course>
};

export type Reason = {
  id?: string,
  name: string,
  deleted: boolean
};

export type Teacher = {
  teacherID: string,
  teacherEmail: string
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
  courses: [Course],
  reasons: [Reason],
  inTime: string,
  outTime: string,
  tutoring: string,
  type: string
};
