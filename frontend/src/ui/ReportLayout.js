import styled from 'styled-components';

export default styled.div`
  display: grid;
  grid-template: 'form report' 1fr / auto 1fr;
  grid-gap: 30px;
  align-items: flex-start;

  @media (max-width: 1350px) {
    justify-content: center;
    grid-template:
      'form' auto
      'report' 1fr / auto;
  }
`;
