import React from 'react';
import styled from 'styled-components';

const Input = ({
  field, // { name, value, onChange, onBlur }
  form: { errors }, // also values, setXXXX, handleXXXX, dirty, isValid, status, etc.
  className,
  label,
  type,
  id,
  ...props
}) => (
  <div className={className}>
    <label htmlFor={id} style={{ textTransform: 'capitalize' }}>
      {label || field.name}
      {errors[field.name] && (
        <div style={{ color: 'red' }}>{errors[field.name]}</div>
      )}
    </label>
    <input id={id} type={type || 'text'} {...field} {...props} />
  </div>
);

export default styled(Input)`
  & input,
  label {
    display: block;
    width: 100%;
    margin-bottom: 10px;
    color: #444;
  }
  & input {
    margin-bottom: 20px;
    border-radius: 5px;
    border: 2px solid #ccc;
    padding: 0.5rem;
  }
  & input:focus {
    outline: 0;
    border: 2px solid ${props => props.theme.color.primary};
  }
`;
