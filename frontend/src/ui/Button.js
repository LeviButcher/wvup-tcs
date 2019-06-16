import styled from 'styled-components';

export default styled.button`
  display: ${props => props.display};
  border-radius: 5px;
  padding: 0.75rem 1.75rem;
  border: none;
  background-color: #4646da;
  color: white;
  &[disabled='true'] {
    background-color: #bfbfff;
  }
`;
