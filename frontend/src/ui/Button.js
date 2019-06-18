import styled from 'styled-components';
import marginConvertor from '../utils/marginConvertor';

export default styled.button.attrs(props => ({
  margin: marginConvertor(props.align)
}))`
  margin: ${props => props.margin};
  display: block;
  border-radius: 5px;
  padding: 0.75rem 1.75rem;
  border: none;

  background-color: #4646da;
  color: white;
  &[disabled='true'] {
    background-color: #bfbfff;
  }
`;
