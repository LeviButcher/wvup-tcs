import React from 'react';
import { Router } from '@reach/router';
import { ThemeProvider } from 'styled-components';
import {
  KioskLayout,
  Home,
  SignOut,
  SignInTeacher,
  SignInStudent
} from './pages/kiosk';
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
  SuccessReport,
  ReasonManagement,
  SignInLookup,
  Welcome,
  SemesterSignIns,
  CreateClassTour,
  UpdateClassTour,
  CreateUser,
  UpdateUser,
  CreateReason,
  UpdateReason,
  CreateSignIn,
  UpdateSignIn
} from './pages/dashboard';
import Theme from './theme.json';
import NotFound from './pages/NotFound';
import IsAuthenticated from './components/IsAuthenticated';

function App() {
  return (
    <>
      <ThemeProvider theme={Theme}>
        <Router>
          <KioskLayout path="/">
            {/* $FlowFixMe */}
            <Home path="/" />
            <SignInStudent path="/signin/*" />
            <SignOut path="/signout" />
            <SignInTeacher path="/signin/teacher" />
            <NotFound default />
          </KioskLayout>
          <IsAuthenticated redirectRoute="/login" path="/dashboard">
            <DashboardLayout path="/">
              <Welcome path="/" />
              {/* // $FlowFixMe */}
              <SignInLookup path="/signins/*" />
              <CreateSignIn path="/signins/create" />
              {/* // $FlowFixMe */}
              <UpdateSignIn path="/signins/:id" />
              {/* // $FlowFixMe */}
              <SemesterSignIns path="/signins/semester/*" />
              {/* // $FlowFixMe */}
              <ClassTourLookup path="/tours/" />
              {/* // $FlowFixMe */}
              <ClassTourLookup path="/tours/:startDate/:endDate" />
              {/* // $FlowFixMe */}
              <ClassTourLookup path="/tours/:startDate/:endDate/:page" />
              <CreateClassTour path="/tours/create" />
              {/* // $FlowFixMe */}
              <UpdateClassTour path="/tours/update/:id" />
              {/* // $FlowFixMe */}
              <ClassTourReport path="/report/tours/*" />
              {/* // $FlowFixMe */}
              <VolunteerReport path="/report/volunteer/*" />
              {/* // $FlowFixMe */}
              <WeeklyVisitsReport path="/report/weekly-visits/*" />
              {/* // $FlowFixMe */}
              <PeakHoursReport path="/report/peak-hours/*" />
              {/* // $FlowFixMe */}
              <ReasonForVisitingReport path="/report/reason-for-visiting/*" />
              <UserManagement path="/admin/users" />
              <CreateUser path="/admin/users/create" />
              {/* $FlowFixMe */}
              <UpdateUser path="/admin/users/update/:id" />
              {/* // $FlowFixMe */}
              <SuccessReport path="/report/success/*" />
              <ReasonManagement path="/admin/reason" />
              <CreateReason path="/admin/reason/create" />
              {/* $FlowFixMe */}
              <UpdateReason path="/admin/reason/update/:id" />
              <NotFound default />
            </DashboardLayout>
          </IsAuthenticated>
          <Login path="/login" />
        </Router>
      </ThemeProvider>
    </>
  );
}

export default App;
