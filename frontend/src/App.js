import React from 'react';
import { Router } from '@reach/router';
import { ThemeProvider } from 'styled-components';
import { KioskLayout, Home, SignIn } from './components/kiosk';
import { DashboardLayout, ClassTourLookup } from './components/dashboard';
import Theme from './theme.json';
import NotFound from './components/NotFound';

function App() {
  return (
    <div>
      <ThemeProvider theme={Theme}>
        <Router>
          <KioskLayout path="/">
            <Home path="/" />
            <SignIn path="/signin" />
            <NotFound default />
          </KioskLayout>
          <DashboardLayout path="/dashboard">
            <Hello path="/" />
            <ClassTourLookup path="/tours" />
            <NotFound default />
          </DashboardLayout>
        </Router>
      </ThemeProvider>
    </div>
  );
}

const Hello = () => <div>Yup</div>;

export default App;
