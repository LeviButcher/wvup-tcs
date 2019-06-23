import React from 'react';
import styled from 'styled-components';

const Header = ({ className, children, type = 'h1' }) => {
  let Heading;
  switch (type) {
    case 'h6':
      Heading = <h6>{children}</h6>;
      break;
    case 'h5':
      Heading = <h5>{children}</h5>;
      break;
    case 'h4':
      Heading = <h4>{children}</h4>;
      break;
    case 'h3':
      Heading = <h3>{children}</h3>;
      break;
    case 'h2':
      Heading = <h2>{children}</h2>;
      break;
    case 'h1':
    default:
      Heading = <h1>{children}</h1>;
  }

  return <header className={className}>{Heading}</header>;
};

export default styled(Header)`
  text-align: ${props => props.align};
`;
