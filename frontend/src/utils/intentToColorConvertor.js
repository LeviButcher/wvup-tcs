import theme from '../theme.json';

type intent = 'primary' | 'secondary' | 'danger' | 'warning';

export default (type: intent) => {
  switch (type) {
    case 'primary':
      return theme.color.primary;
    case 'secondary':
      return theme.color.secondary;
    case 'danger':
      return theme.color.danger;
    case 'warning':
      return theme.color.warning;
    default:
      return '#4646da';
  }
};
