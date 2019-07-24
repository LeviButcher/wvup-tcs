import styled from 'styled-components';

export default styled.table`
  width: 100%;
  border-collapse: collapse;

  & > thead {
    background: ${props => props.theme.color.main};
    color: ${props => props.theme.color.accent};
  }
  & > tbody tr:nth-child(odd) {
    background: #eee;
  }
  & > tbody tr:nth-child(even) {
    background: #ccc;
  }
  & th {
    padding: 1rem;
  }
  & > tbody tr:hover {
    background: ${props => props.theme.color.accent};
    color: ${props => props.theme.color.main};
  }
  & td {
    padding: 0.75rem 1rem;
  }
  & tfoot {
    border-top: 2px solid ${props => props.theme.color.main};
    border-bottom: 2px solid ${props => props.theme.color.main};
  }
`;
