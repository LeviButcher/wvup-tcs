import React from 'react';
import { Field } from 'formik';
import { Header, FieldGroup, Checkbox, Stack, SmallText } from '../ui';
import { StyledCheckbox } from '../ui/Checkbox';
import type { Reason } from '../types';

type Props = {
  className?: string,
  reasons: Array<Reason>,
  values: any,
  errors: any,
  touched: any
};

const ReasonsCheckboxes = ({
  className,
  reasons,
  values,
  errors,
  touched
}: Props) => (
  <Stack className={className}>
    <Header type="h4">
      Reason for Visiting{' '}
      <SmallText>Select Tutoring or at least one other reason</SmallText>
      {errors.reasons && touched.reasons && (
        <div style={{ color: 'red' }}>{errors.reasons}</div>
      )}
    </Header>
    <FieldGroup>
      <StyledCheckbox name="tutoring" data-checked={values.tutoring}>
        Tutoring
        <Field
          id="tutoring"
          type="checkbox"
          name="tutoring"
          value="Tutoring"
          checked={values.tutoring}
        >
          {({ form, ...rest }) => {
            return (
              <input
                {...rest}
                checked={values.tutoring}
                type="checkbox"
                onChange={() => {
                  form.setFieldValue('tutoring', !values.tutoring);
                }}
              />
            );
          }}
        </Field>
      </StyledCheckbox>
      {reasons.map(reason => (
        <Checkbox
          key={reason.id}
          id={reason.name}
          type="checkbox"
          name="reasons"
          label={`${reason.name}`}
          value={reason.id || ''}
          data-deleted={reason.deleted}
          title={`This reason is ${reason.deleted ? 'deleted' : 'active'}`}
        />
      ))}
    </FieldGroup>
  </Stack>
);

ReasonsCheckboxes.defaultProps = {
  className: ''
};

export default ReasonsCheckboxes;
