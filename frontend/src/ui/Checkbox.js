import React from 'react';
import { Field } from 'formik';
import styled from 'styled-components';

type Props = {
  name: string,
  value: string,
  label: string,
  id: string
};

export default function Checkbox({ name, value, label, id, ...rest }: Props) {
  return (
    <Field name={name}>
      {({ field, form }) => (
        <StyledCheckbox
          name={name}
          htmlFor={id}
          {...rest}
          data-checked={field.value ? field.value.includes(value) : false}
        >
          {label}
          <input
            type="checkbox"
            id={id}
            name={name}
            label={label}
            {...rest}
            checked={field.value ? field.value.includes(value) : false}
            onChange={() => {
              if (field.value && field.value.includes(value)) {
                const nextValue = field.value.filter(
                  oldValue => oldValue !== value
                );
                form.setFieldValue(name, nextValue);
              } else {
                const nextValue = field.value.concat(value);
                form.setFieldValue(name, nextValue);
              }
            }}
          />
        </StyledCheckbox>
      )}
    </Field>
  );
}

const StyledCheckbox = styled.label`
  & input {
    display: none;
  }

  color: #444;
  background: #fff;
  border: 1px solid #444;
  border-radius: 5px;
  padding: 1rem;
  text-align: center;
  transition: background 0.25s, color 0.25s, border 0.25s;

  &:active {
    transform: scale(0.98);
  }

  &[data-checked='true'] {
    background: ${props => props.theme.color.primary};
    color: white;
    border: 1px solid transparent;
  }
`;

export { StyledCheckbox };
