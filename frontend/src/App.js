import React from 'react';
import { Router } from '@reach/router';
import { ThemeProvider } from 'styled-components';
import { KioskLayout, Home, SignIn } from './components/kiosk';
import { DashboardLayout } from './components/dashboard';
import Theme from './theme.json';

function App() {
  return (
    <div>
      <ThemeProvider theme={Theme}>
        <Router>
          <KioskLayout path="/">
            <Home path="/" />
            <SignIn path="/signin" />
          </KioskLayout>
          <DashboardLayout path="/dashboard">
            <Hello path="/" />
          </DashboardLayout>
        </Router>
      </ThemeProvider>
    </div>
  );
}

const Hello = () => <div>Yup</div>;

export default App;
