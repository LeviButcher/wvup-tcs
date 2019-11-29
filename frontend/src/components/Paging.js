import React from 'react';
import styled from 'styled-components';
import { Link, Button } from '../ui';

type Props = {
  className?: string,
  currentPage: string,
  totalPages: string,
  basePath: string
};

const PagingNav = styled.nav`
  margin: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  & > * {
    margin: 0 2rem !important;
  }
`;

const DisabledLink = styled(Link)`
  pointer-events: ${props => (props['aria-disabled'] ? 'none' : 'all')};
`;

const Paging = ({
  className,
  currentPage = '1',
  totalPages = '1',
  basePath
}: Props) => {
  const page = Number(currentPage);
  return (
    <PagingNav className={className} aria-label="Pagination Navigation">
      <DisabledLink
        to={`${basePath}/${page - 1}${window.location.search}`}
        aria-label="Previous Page"
        aria-disabled={page <= 1}
      >
        <Button variant="outlined" color="secondary" disabled={page <= 1}>
          Prev
        </Button>
      </DisabledLink>
      <p>
        {page} of {totalPages}
      </p>
      <DisabledLink
        to={`${basePath}/${page + 1}${window.location.search}`}
        aria-label="Next Page"
        aria-disabled={page >= Number(totalPages)}
      >
        <Button
          variant="contained"
          color="primary"
          disabled={page >= Number(totalPages)}
        >
          Next
        </Button>
      </DisabledLink>
    </PagingNav>
  );
};

Paging.defaultProps = {
  className: ''
};

export default Paging;
