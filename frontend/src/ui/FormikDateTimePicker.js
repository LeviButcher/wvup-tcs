import React from 'react';
import DateTimePicker from 'react-datetime-picker';
import styled from 'styled-components';

const FormikDateTimePicker = ({
  field, // { name, value, onChange, onBlur }
  form: { errors, setFieldValue }, // also values, setXXXX, handleXXXX, dirty, isValid, status, etc.
  className,
  label,
  id
}) => (
  <div className={className}>
    <label htmlFor={id}>{label || field.name}</label>
    {errors[field.name] && (
      <div style={{ color: 'red' }}>{errors[field.name]}</div>
    )}
    <DateTimePicker
      {...field}
      value={
        !Number.isNaN(Date.parse(field.value)) ? new Date(field.value) : ''
      }
      onChange={e => {
        setFieldValue(field.name, e);
      }}
    />
  </div>
);

export default styled(FormikDateTimePicker)`
  label {
    display: block;
    margin: 0.5rem 0;
  }
`;
