import styled from 'styled-components';

export default styled.div`
  display: grid;
  grid-template: 'a a a' auto / 1fr 1fr 1fr;
  grid-gap: 1rem;
  & > div {
    display: flex;
  }
`;
