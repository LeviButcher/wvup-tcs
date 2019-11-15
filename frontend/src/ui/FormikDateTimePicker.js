import React from 'react';
import DateTimePicker from 'react-datetime-picker';
import styled from 'styled-components';
import SmallText from './SmallText';

const isDate = d => !Number.isNaN(Date.parse(d));

const FormikDateTimePicker = ({
  field, // { name, value, onChange, onBlur }
  form: { errors, setFieldValue, touched, setTouched }, // also values, setXXXX, handleXXXX, dirty, isValid, status, etc.
  className,
  label,
  id
}) => {
  return (
    <div className={className}>
      <label htmlFor={id}>{label || field.name}</label>
      {errors[field.name] && touched[field.name] && (
        <div>
          {' '}
          <SmallText style={{ color: 'red' }}> {errors[field.name]}</SmallText>
        </div>
      )}
      <DateTimePicker
        id={id}
        {...field}
        value={isDate(field.value) ? new Date(field.value) : ''}
        onChange={e => {
          const date = e || '';
          setFieldValue(field.name, date.toString());
          const isTouched = { [field.name]: true };
          setTouched(isTouched);
        }}
      />
    </div>
  );
};

export default styled(FormikDateTimePicker)`
  label {
    display: block;
    margin: 0.5rem 0;
  }
`;
