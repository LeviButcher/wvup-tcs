// Converts char to key code used for fire Events
const toKeyCode = (char: string) => {
  switch (char) {
    case 'a':
      return { key: 'a', code: '' };
    case 'b':
      return { key: 'b', code: 85 };
    case 'l':
      return { key: 'l', code: 66 };
    case '9':
      return { key: '9', code: 84 };
    case '8':
      return { key: '8', code: 84 };
    case '?':
      return { key: '?', code: 84 };
    case ';':
      return { key: ';', code: 67 };
    case '\n':
      return { key: 'Enter', code: 13 };
    default:
      return { key: ':(', code: 'char could not be found' };
  }
};

export default toKeyCode;
