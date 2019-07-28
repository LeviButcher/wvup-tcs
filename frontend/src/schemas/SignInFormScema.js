import * as Yup from 'yup';

const SignInSchema = Yup.object().shape({
  email: Yup.string()
    .email('Invalid email')
    .matches(/^[A-Z0-9._%+-]+@wvup.edu$/i, 'Must be a wvup email address')
    .trim()
    .required('Email is required'),
  courses: Yup.array()
    .min(1, 'You must select at least 1 course')
    .required('A Course is required'),
  reasons: Yup.array().when('tutoring', {
    is: true,
    then: Yup.array(),
    otherwise: Yup.array()
      .min(1, 'You must select at least 1 reason')
      .required()
  }),
  tutoring: Yup.boolean()
});

export default SignInSchema;
