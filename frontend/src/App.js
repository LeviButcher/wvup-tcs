import React from 'react';
import { Router } from '@reach/router';
import { ThemeProvider } from 'styled-components';
import { KioskLayout, Home, SignIn, SignOut } from './components/kiosk';
import {
  DashboardLayout,
  ClassTourLookup,
  Login,
  ClassTourReport,
  VolunteerReport,
  WeeklyVisitsReport,
  PeakHoursReport,
  ReasonForVisitingReport
} from './components/dashboard';
import Theme from './theme.json';
import NotFound from './components/NotFound';
import ClassTourForm from './components/dashboard/ClassTourForm';
import FetchClassTour from './components/dashboard/FetchClassTour';
import IsAuthenticated from './components/IsAuthenticated';

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
          <IsAuthenticated redirectRoute="/login" path="/dashboard">
            <DashboardLayout path="/">
              <Hello path="/" />
              <ClassTourLookup path="/tours" />
              <ClassTourForm path="/tours/create" />
              <FetchClassTour
                path="/tours/update/:classTourId"
                Component={ClassTourForm}
                action="Update"
              />
              <ClassTourReport path="/report/tours" />
              <VolunteerReport path="/report/volunteer" />
              <WeeklyVisitsReport path="/report/weekly-visits" />
              <PeakHoursReport path="/report/peak-hours" />
              <ReasonForVisitingReport path="/report/reason-for-visiting" />
              <NotFound default />
            </DashboardLayout>
          </IsAuthenticated>
          <Login path="/login" />
        </Router>
      </ThemeProvider>
    </div>
  );
}

const Hello = () => (
  <div>Hey Levi you need to make this default dashboard view</div>
);

export default App;
