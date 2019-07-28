import React from 'react';
import { Field } from 'formik';
import styled from 'styled-components';
import { Header, FieldGroup, Checkbox } from '../ui';

const ReasonsCheckboxes = ({ className, reasons, values, errors }) => (
  <div className={className}>
    <Header type="h4">
      Reason for Visiting{' '}
      <SmallText>Select Tutoring or at least one other reason</SmallText>
      <div style={{ color: 'red' }}>{errors && errors.reasons}</div>q
    </Header>
    <FieldGroup>
      <SingleCheckBoxLabel name="tutoring">
        Tutoring
        <Field
          id="tutoring"
          type="checkbox"
          name="tutoring"
          component="input"
          label="tutoring"
          value="Tutoring"
          checked={values.tutoring}
        />
      </SingleCheckBoxLabel>
      {reasons.map(reason => (
        <Checkbox
          key={reason.id}
          id={reason.name}
          type="checkbox"
          name="reasons"
          label={`${reason.name}`}
          value={reason.id}
          data-deleted={reason.deleted}
          title={`This reason is ${reason.deleted ? 'deleted' : 'active'}`}
        />
      ))}
    </FieldGroup>
  </div>
);

const SmallText = styled.span`
  color: #aaa;
  font-size: 0.8em;
`;

const SingleCheckBoxLabel = styled.label`
  display: flex;
  justify-content: space-between;
  align-items: center;
`;

export default ReasonsCheckboxes;
