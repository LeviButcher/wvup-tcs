import React from 'react';
import styled from 'styled-components';
import Stack from './Stack';
import SmallText from './SmallText';

const Input = ({
  field, // { name, value, onChange, onBlur }
  form: { errors, touched }, // also values, setXXXX, handleXXXX, dirty, isValid, status, etc.
  className,
  label,
  type,
  id,
  hidden,
  ...props
}) => {
  return (
    <Stack size="small" className={className} hidden={hidden} data-type={type}>
      <label htmlFor={id} style={{ textTransform: 'capitalize' }}>
        {label || field.name}
        {errors[field.name] && touched[field.name] && (
          <SmallText style={{ color: 'red' }}> {errors[field.name]}</SmallText>
        )}
      </label>
      <input id={id} type={type || 'text'} {...field} {...props} />
    </Stack>
  );
};

export default styled(Input)`
  & input,
  label {
    display: block;
    width: 100%;
    color: #444;
  }
  & input {
    border-radius: 5px;
    border: 2px solid #ccc;
    padding: 0.5rem;
  }
  & input:focus {
    outline: 0;
    border: 2px solid ${props => props.theme.color.primary};
  }
  &[data-type='checkbox'] {
    display: flex;
    align-items: center;
    & input,
    label {
      width: auto;
      margin-top: auto;
    }
    & input {
      margin-left: 1rem;
    }
  }
  &[hidden] {
    display: none;
  }
`;
