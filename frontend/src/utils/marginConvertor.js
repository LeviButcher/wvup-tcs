function marginConvertor(align) {
  let margin = 'auto';
  switch (align) {
    case 'left':
      margin = '0 auto 0 0';
      break;
    case 'right':
      margin = '0 0 0 auto';
      break;
    default:
  }
  return margin;
}

export default marginConvertor;
