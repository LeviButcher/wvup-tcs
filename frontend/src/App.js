import React from 'react';
import { Router, navigate } from '@reach/router';
import { ThemeProvider } from 'styled-components';
import { KioskLayout, Home, SignOut, SignInTeacher } from './components/kiosk';
import {
  DashboardLayout,
  ClassTourLookup,
  Login,
  ClassTourReport,
  VolunteerReport,
  WeeklyVisitsReport,
  PeakHoursReport,
  ReasonForVisitingReport,
  UserManagement,
  UserForm,
  SuccessReport,
  ReasonManagement,
  ReasonForm,
  SignInLookup,
  SignInForm
} from './components/dashboard';
import Theme from './theme.json';
import NotFound from './components/NotFound';
import ClassTourForm from './components/dashboard/ClassTourForm';
import Fetch from './components/Fetch';
import IsAuthenticated from './components/IsAuthenticated';

function App() {
  return (
    <div>
      <ThemeProvider theme={Theme}>
        <Router>
          <KioskLayout path="/">
            <Home path="/" />
            <SignInForm
              path="/signin"
              afterSuccessfulSubmit={() => {
                alert('You have signed in! ');
                navigate('/');
              }}
            />
            <SignOut path="/signout" />
            <SignInTeacher path="/signin/teacher" />
            <NotFound default />
          </KioskLayout>
          <IsAuthenticated redirectRoute="/login" path="/dashboard">
            <DashboardLayout path="/">
              <Hello path="/" />
              <SignInLookup path="/signins/:startDate/:endDate/:page" />
              <SignInLookup path="/signins/" />
              <SignInForm
                path="/signins/create"
                afterSuccessfulSubmit={() => {
                  alert('You created a signIn! ');
                  navigate('/dashboard/signins');
                }}
              />
              <ClassTourLookup path="/tours" />
              <ClassTourForm path="/tours/create" />
              <Fetch
                url={`${process.env.REACT_APP_BACKEND}classtours/`}
                path="/tours/update/:id"
                Component={ClassTourForm}
                action="Update"
              />
              <ClassTourReport path="/report/tours" />
              <VolunteerReport path="/report/volunteer" />
              <WeeklyVisitsReport path="/report/weekly-visits" />
              <PeakHoursReport path="/report/peak-hours" />
              <ReasonForVisitingReport path="/report/reason-for-visiting" />
              <UserManagement path="/admin/users" />
              <UserForm path="/admin/users/create" />
              <Fetch
                url={`${process.env.REACT_APP_BACKEND}users/`}
                path="/admin/users/update/:id"
                Component={UserForm}
                action="Update"
              />
              <SuccessReport path="/report/success" />
              <ReasonManagement path="/admin/reason" />
              <ReasonForm path="/admin/reason/create" />
              <Fetch
                url={`${process.env.REACT_APP_BACKEND}reasons/`}
                path="/admin/reason/update/:id"
                Component={ReasonForm}
                action="Update"
              />
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
