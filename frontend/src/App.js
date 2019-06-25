import React from 'react';
import { Router } from '@reach/router';
import { ThemeProvider } from 'styled-components';
import { KioskLayout, Home, SignIn, SignOut } from './components/kiosk';
import {
  DashboardLayout,
  ClassTourLookup,
  Login
} from './components/dashboard';
import Theme from './theme.json';
import NotFound from './components/NotFound';
import ClassTourForm from './components/dashboard/ClassTourForm';
import FetchClassTour from './components/dashboard/FetchClassTour';

function App() {
  return (
    <div>
      <ThemeProvider theme={Theme}>
        <Router>
          <KioskLayout path="/">
            <Home path="/" />
            <SignIn path="/signin" />
            <SignOut path="/signout" />
            <NotFound default />
          </KioskLayout>
          <DashboardLayout path="/dashboard">
            <Hello path="/" />
            <ClassTourLookup path="/tours" />
            <ClassTourForm path="/tours/create" />
            <FetchClassTour
              path="/tours/update/:classTourId"
              Component={ClassTourForm}
            />
            <NotFound default />
          </DashboardLayout>
          <Login path="/login" />
        </Router>
      </ThemeProvider>
    </div>
  );
}

const Hello = () => <div>Yup</div>;

export default App;
