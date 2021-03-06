import styled from 'styled-components';
import marginConvertor from '../utils/marginConvertor';
import intentToColorConvertor from '../utils/intentToColorConvertor';

export default styled.button.attrs(props => ({
  margin: marginConvertor(props.align),
  background: intentToColorConvertor(props.intent)
}))`
  margin: ${props => props.margin};
  display: ${props => props.display || 'block'};
  border-radius: 5px;
  padding: 0.75rem 1.75rem;
  border: none;
  cursor: pointer;
  text-decoration: none !important;
  background: ${props => props.background};
  color: white;
  width: ${props => (props.fullWidth ? '100%' : 'auto')};

  &[disabled] {
    background: ${props => props.background}77;
  }
  &:hover {
    box-shadow: 0 0 10px 1px ${props => props.background}bb;
  }
  &[disabled]:hover {
    box-shadow: none;
  }
  & > a {
    color: white;
    text-decoration: none;
  }
`;
