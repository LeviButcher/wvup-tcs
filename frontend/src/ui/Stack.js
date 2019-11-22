import styled from 'styled-components';

const stackSize = (size: 'small' | 'large') => {
  switch (size) {
    case 'small':
      return '1rem';
    case 'large':
      return '3rem';
    default:
      return '2rem';
  }
};

const Stack = styled.div`
  & > * + * {
    margin-top: ${props => stackSize(props.size)} !important;
  }
`;

export default Stack;
