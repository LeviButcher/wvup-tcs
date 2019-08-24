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
  UserForm,
  SuccessReport,
  ReasonManagement,
  ReasonForm,
  SignInLookup,
  SignInFormUpdate,
  Welcome,
  SemesterSignIns
} from './pages/dashboard';
import Theme from './theme.json';
import NotFound from './pages/NotFound';
import ClassTourForm from './pages/dashboard/ClassTourForm';
import Fetch from './components/Fetch';
import IsAuthenticated from './components/IsAuthenticated';

function App() {
  return (
    <>
      <ThemeProvider theme={Theme}>
        <Router>
          <KioskLayout path="/">
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
              <ClassTourLookup path="/tours/*" />
              <ClassTourForm path="/tours/create" />
              <Fetch
                url="classtours/"
                path="/tours/update/:id"
                Component={ClassTourForm}
                action="Update"
              />
              <ClassTourReport path="/report/tours/*" />
              <VolunteerReport path="/report/volunteer/*" />
              <WeeklyVisitsReport path="/report/weekly-visits/*" />
              <PeakHoursReport path="/report/peak-hours/*" />
              <ReasonForVisitingReport path="/report/reason-for-visiting/*" />
              <UserManagement path="/admin/users" />
              <UserForm path="/admin/users/create" />
              <Fetch
                url="users/"
                path="/admin/users/update/:id"
                Component={UserForm}
                action="Update"
              />
              <SuccessReport path="/report/success/*" />
              <ReasonManagement path="/admin/reason" />
              <ReasonForm path="/admin/reason/create" />
              <Fetch
                url="reasons/"
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
    </>
  );
}

export default App;
