import React from 'react';
import styled from 'styled-components';
import Link from './Link';

const Paging = ({ className, next, prev, currentPage = 1, baseURL }) => {
  const page = Number(currentPage);
  return (
    <nav className={className} aria-label="Pagination Navigation">
      {page > 1 && prev && (
        <Link to={`${baseURL}${page - 1}`} aria-label="Previos Page">
          Prev
        </Link>
      )}
      <i>{page} of ?</i>
      {next && (
        <Link to={`${baseURL}${page + 1}`} aria-label="Next Page">
          Next
        </Link>
      )}
    </nav>
  );
};

export default styled(Paging)`
  padding: 1rem;
  margin: 1rem;
  text-align: center;
  font-size: 1.2rem;

  & > a {
    padding: 0.75rem;
    border-radius: 5px;
    text-decoration: none;
    background: ${props => props.theme.color.main};
    color: white;

    &:hover {
      color: ${props => props.theme.color.accent};
    }
  }

  & > i {
    padding: 1rem;
  }
`;
