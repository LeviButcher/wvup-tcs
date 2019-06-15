import React from 'react';
import { Router } from '@reach/router';
import { KioskLayout, Home, SignIn } from './components/kiosk';
import { DashboardLayout } from './components/dashboard';

function App() {
  return (
    <div>
      <Router>
        <KioskLayout path="/">
          <Home path="/" />
          <SignIn path="/signin" />
        </KioskLayout>
        <DashboardLayout path="/dashboard">
          <Hello path="/" />
        </DashboardLayout>
      </Router>
    </div>
  );
}

const Hello = () => <div>Yup</div>;

export default App;
