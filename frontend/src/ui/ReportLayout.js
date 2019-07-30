import styled from 'styled-components';

export default styled.div`
  display: grid;
  align-items: start;
  flex-flow: wrap row;
  justify-items: start;

  grid-template:
    'form table' auto
    'chart table' 1fr / auto 1fr;

  /* need to fix this media queries */
  @media (max-width: 1350px) {
    align-items: start;
    justify-items: center;
    grid-template:
      'form chart' auto
      'table table' auto / 1fr 2fr;
  }

  @media (max-width: 800px) {
    align-items: center;
    justify-items: center;
    grid-template:
      'form' auto
      'chart' auto
      'table' auto / 1fr 2fr;
  }
`;
