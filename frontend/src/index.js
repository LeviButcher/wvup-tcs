import React from 'react';
import ReactDOM from 'react-dom';
import 'normalize.css';
import './index.css';
import App from './App';

const root = document.getElementById('root');

if (root) {
  ReactDOM.render(<App />, root);
} else {
  // eslint-disable-next-line no-console
  console.error("Can't find root element");
}
