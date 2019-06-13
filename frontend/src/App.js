import React from 'react';
import { Router } from '@reach/router';
import { KioskLayout, Home, SignIn } from './components/kiosk';
import './App.css';

function App() {
  return (
    <div>
      <Router>
        <KioskLayout path="/">
          <Home path="/" />
          <SignIn path="/signin" />
        </KioskLayout>
      </Router>
    </div>
  );
}

export default App;
