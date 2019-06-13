import React from 'react';

const Input = ({
  field, // { name, value, onChange, onBlur }
  form: { touched, errors }, // also values, setXXXX, handleXXXX, dirty, isValid, status, etc.
  ...props
}) => (
  <div>
    <label style={{ 'text-transform': 'capitalize' }}>
      {props.label || field.name}
      <input type="text" {...field} {...props} />
    </label>
    {touched[field.name] && errors[field.name] && (
      <div className="error">{errors[field.name]}</div>
    )}
  </div>
);

export default Input;
