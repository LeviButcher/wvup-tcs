﻿using System;
using System.Collections.Generic;
using System.Linq;
using tcs_service.Models;
using tcs_service.Models.DTO;

namespace tcs_service.Services {

    /// <summary>All Business Logic to generate the various reports for TCS</summary>
    public class ReportsBusinessLogic {

        /// <summary>Returns the amount of visitors during the weeks in between the start and end date</summary>
        public static List<WeeklyVisitsDTO> WeeklyVisits (IEnumerable<Session> sessions, DateTime start, DateTime end) {
            var result = new List<WeeklyVisitsDTO> ();
            while (start <= end) {
                result.Add (new WeeklyVisitsDTO (start, start.Date.AddDays (6)) {
                    Count = sessions.Where (x => x.InTime.Date >= start.Date && x.InTime.Date <= start.AddDays (6)).Count ()
                });
                start = start.AddDays (7);
            }
            return result;
        }

        /// <summary>Returns the amount of students that visited during various hours between a start and end date</summary>
        public static List<PeakHoursDTO> PeakHours (IEnumerable<Session> sessions, DateTime start, DateTime end) =>
            sessions.Where (x => x.InTime.Date >= start.Date && x.InTime.Date <= end.Date)
            .GroupBy (x => x.InTime.Hour)
            .Where (x => x.Count () >= 1)
            .Select (x => new PeakHoursDTO (x.Key, x.Count ())).ToList ();

        /// <summary>Returns the total amount of visitors each class tour had between a start and end date</summary>
        public static List<ClassTourReportDTO> ClassTours (IEnumerable<ClassTour> tours, DateTime start, DateTime end) =>
            tours.Where (x => x.DayVisited >= start && x.DayVisited <= end)
            .GroupBy (x => x.Name)
            .Select (x => new ClassTourReportDTO { Name = x.Key, Students = x.Sum (s => s.NumberOfStudents) })
            .ToList ();

        /// <summary>Returns the amount of hours each teacher has volunteered to the center during a start and end date</summary>
        public static List<TeacherSignInTimeDTO> Volunteers (IEnumerable<Session> sessions, DateTime start, DateTime end) {
            var teachers = from session in sessions
            where session.InTime.Date >= start.Date &&
                session.InTime.Date <= end.Date &&
                session.Person.PersonType == PersonType.Teacher
            select new {
                fullName = $"{session.Person.FirstName} {session.Person.LastName}",
                teacherEmail = session.Person.Email,
                totalHours = Convert.ToDecimal (session.OutTime.Value.Ticks) - Convert.ToDecimal (session.InTime.Ticks)
            };

            var result = from item in teachers
            group item by new {
                item.teacherEmail,
                item.fullName
                }
                into grp
                select new TeacherSignInTimeDTO () {
                FullName = grp.Key.fullName,
                TeacherEmail = grp.Key.teacherEmail,
                TotalHours = Math.Round (grp.Sum (x => x.totalHours / 36000000000), 2) //36000000000 = microseconds in an hour
            };

            return result.ToList ();
        }

        /// <summary>Returns the amount of time a person has come in for a class and a reason between a start and end date</summary>
        public static List<ReasonWithClassVisitsDTO> Reasons (IEnumerable<Session> sessions, DateTime start, DateTime end) {
            var result = from session in sessions
            from reason in session.SessionReasons
            where reason.Reason.Name != "Tutoring"
            from course in session.SessionClasses
            where session.InTime.Date >= start.Date &&
                session.InTime.Date <= end.Date
            select new {
                ReasonName = reason.Reason.Name,
                reason.ReasonId,
                className = course.Class.Name,
                classId = course.ClassId
            };

            var tutoringResult = from session in sessions
            from course in session.SessionClasses
            where session.Tutoring == true &&
                session.InTime.Date >= start.Date &&
                session.InTime.Date <= end.Date
            select new {

                className = course.Class.Name,
                classId = course.ClassId
            };

            var resultGroup = from item in result
            group item by new {
                item.classId,
                item.ReasonId,
                item.className,
                item.ReasonName
                }
                into grp
                select new ReasonWithClassVisitsDTO () {
                ReasonId = grp.Key.ReasonId,
                ReasonName = grp.Key.ReasonName,
                ClassCRN = grp.Key.classId,
                ClassName = grp.Key.className,
                Visits = grp.Count ()
            };

            var tutorResult = from item in tutoringResult
            group item by new {
                item.classId,
                item.className
                }
                into grp
                select new ReasonWithClassVisitsDTO () {
                ReasonId = 0,
                ReasonName = "Tutoring",
                ClassCRN = grp.Key.classId,
                ClassName = grp.Key.className,
                Visits = grp.Count ()
            };

            var finalResult = resultGroup.Concat (tutorResult);

            return finalResult.ToList ();
        }

        /// <summary>Returns the amount of people that passed, dropped, or completed each class that had been visited during a semester</summary>
        public static List<ClassSuccessCountDTO> SuccessReport (IEnumerable<ClassWithGradeDTO> classesWithGrades) {
            var cleanedClassGrades = classesWithGrades.Where (x => x != null);

            return cleanedClassGrades
                .Select (x => new { x.CRN, x.CourseName, x.DepartmentName })
                .Distinct ()
                .Select (x => new ClassSuccessCountDTO (x.CRN, x.CourseName, x.DepartmentName))
                .Select (x =>
                    cleanedClassGrades.Where (c => c.CRN == x.CRN)
                    .Aggregate (x, (acc, curr) => acc.DetermineSuccess (curr.FinalGrade))
                ).ToList ();
        }
    }
}