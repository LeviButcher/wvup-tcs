import styled from 'styled-components';

export default styled.article`
  width: ${props => props.width || '600px'};
  margin: 2rem;
  padding: 2rem;
  box-shadow: 0 0 20px 5px rgba(0, 0, 0, 0.3);
`;
