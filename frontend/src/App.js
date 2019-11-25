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
  TeacherSignInFormUpdate,
  VolunteerReport,
  WeeklyVisitsReport,
  PeakHoursReport,
  ReasonForVisitingReport,
  UserManagement,
  SuccessReport,
  ReasonManagement,
  SignInLookup,
  SignInFormUpdate,
  Welcome,
  SemesterSignIns,
  ClassTourCreate,
  ClassTourUpdate,
  CreateUser,
  UpdateUser,
  CreateReason,
  UpdateReason
} from './pages/dashboard';
import Theme from './theme.json';
import NotFound from './pages/NotFound';
import Fetch from './components/Fetch';
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
              <SignInLookup path="/signins/*" />
              <SignInFormUpdate path="/signins/create" type="create" />
              <SemesterSignIns path="/signins/semester/*" />
              <TeacherSignInFormUpdate
                path="/signins/teacher/create"
                type="create"
              />
              <Fetch
                path="/signins/:id"
                url="signins/"
                Component={SignInFormUpdate}
                action="Update"
                type="update"
              />
              <Fetch
                path="/signins/teacher/:id"
                url="signins/"
                Component={TeacherSignInFormUpdate}
                action="Update"
                type="update"
              />
              {/* // $FlowFixMe */}
              <ClassTourLookup path="/tours/" />
              {/* // $FlowFixMe */}
              <ClassTourLookup path="/tours/:startDate/:endDate" />
              {/* // $FlowFixMe */}
              <ClassTourLookup path="/tours/:startDate/:endDate/:page" />
              <ClassTourCreate path="/tours/create" />
              {/* // $FlowFixMe */}
              <ClassTourUpdate path="/tours/update/:id" />
              <ClassTourReport path="/report/tours/*" />
              <VolunteerReport path="/report/volunteer/*" />
              <WeeklyVisitsReport path="/report/weekly-visits/*" />
              <PeakHoursReport path="/report/peak-hours/*" />
              <ReasonForVisitingReport path="/report/reason-for-visiting/*" />
              <UserManagement path="/admin/users" />
              <CreateUser path="/admin/users/create" />
              {/* $FlowFixMe */}
              <UpdateUser path="/admin/users/update/:id" />
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
