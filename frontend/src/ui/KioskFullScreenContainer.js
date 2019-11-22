import styled from 'styled-components';

const KioskFullScreenContainer = styled.div`
  padding: ${props => props.theme.padding};
  height: calc(100vh - ${props => props.theme.kioskHeaderSize});
  display: flex;
  align-items: center;
  justify-content: space-evenly;
`;

export default KioskFullScreenContainer;
