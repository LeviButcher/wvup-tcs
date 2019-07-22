import styled from 'styled-components';
import { GoTrashcan, GoGear } from 'react-icons/go';

const baseSize = '1.4rem';

const Trashcan = styled(GoTrashcan)`
  color: red;
  font-size: ${baseSize};

  &:hover {
    cursor: pointer;
  }
`;

const Gear = styled(GoGear)`
  font-size: ${baseSize};
`;

export { Trashcan, Gear };
