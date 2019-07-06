import styled from 'styled-components';

export default styled.table`
  width: 100%;
  border-collapse: collapse;

  & > thead {
    background: ${props => props.theme.color.main};
    color: ${props => props.theme.color.accent};
  }
  & > tbody tr:nth-child(even) {
    background: #ccc;
  }
  & th {
    padding: 1rem;
  }
  & > tbody tr:hover {
    background: #ddd;
  }
  & td {
    padding: 0.5rem 1rem;
  }

  & tfoot {
    border-top: 2px solid ${props => props.theme.color.main};
  }
`;
