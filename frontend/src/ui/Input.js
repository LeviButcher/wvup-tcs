import React from 'react';
import styled from 'styled-components';

const Input = ({
  field, // { name, value, onChange, onBlur }
  form: { touched, errors }, // also values, setXXXX, handleXXXX, dirty, isValid, status, etc.
  className,
  label,
  type,
  id,
  ...props
}) => (
  <div className={className}>
    <label htmlFor={id} style={{ textTransform: 'capitalize' }}>
      {label || field.name}
    </label>
    <input id={id} type={type || 'text'} {...field} {...props} />
    {touched[field.name] && errors[field.name] && (
      <div className="error">{errors[field.name]}</div>
    )}
  </div>
);

export default styled(Input)`
  & input,
  label {
    display: block;
    width: 100%;
    margin-bottom: 10px;
  }
  & input {
    margin-bottom: 20px;
  }
`;
