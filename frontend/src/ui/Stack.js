import styled from 'styled-components';

const Stack = styled.div`
  & > * + * {
    margin-bottom: 2rem !important;
  }
`;

export default Stack;
