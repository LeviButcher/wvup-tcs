import React from 'react';
import styled from 'styled-components';

const Header = ({ className, text, type = 'h1' }) => {
  let Heading;
  switch (type) {
    case 'h6':
      Heading = <h6>{text}</h6>;
      break;
    case 'h5':
      Heading = <h5>{text}</h5>;
      break;
    case 'h4':
      Heading = <h4>{text}</h4>;
      break;
    case 'h3':
      Heading = <h3>{text}</h3>;
      break;
    case 'h2':
      Heading = <h2>{text}</h2>;
      break;
    case 'h1':
    default:
      Heading = <h1>{text}</h1>;
  }

  return <header className={className}>{Heading}</header>;
};

export default styled(Header)`
  text-align: ${props => props.align};
`;
