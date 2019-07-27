import styled from 'styled-components';

export default styled.div`
  display: grid;
  align-items: start;
  flex-flow: wrap row;
  justify-items: start;

  grid-template:
    'form table' auto
    'chart table' auto / auto 1fr;

  @media (max-width: 1350px) {
  }
`;
