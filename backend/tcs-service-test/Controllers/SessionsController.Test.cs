using Microsoft.VisualBasic.CompilerServices;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Linq.Expressions;
using tcs_service.Controllers;
using tcs_service.Models;
using tcs_service.Repos;
using tcs_service.Repos.Interfaces;
using tcs_service_test.Helpers;
using Xunit;
using tcs_service.Models.DTOs;
using tcs_service.EF;
using System.Collections.Generic;

namespace tcs_service_test
{
    public class SessionsControllerTest : IDisposable
    {
        const string dbName = "SessionsControllerTest";
        readonly SessionsController sessionController;
        readonly TCSContext db;

        readonly ISessionRepo sessionRepo;

        public SessionsControllerTest()
        {
            var dbOptions = DbInMemory.getDbInMemoryOptions(dbName);
            db = new TCSContext(dbOptions);
            sessionRepo = new SessionRepo(dbOptions);
            var personRepo = new PersonRepo(dbOptions);
            var semesterRepo = new SemesterRepo(dbOptions);
            var sessionClassRepo = new SessionClassRepo(dbOptions);
            var sessionReasonRepo = new SessionReasonRepo(dbOptions);
            var mapper = Helpers.Utils.GetProjectMapper();
            sessionController = new SessionsController(sessionRepo, semesterRepo, personRepo, sessionReasonRepo, sessionClassRepo, mapper);
        }

        public void Dispose()
        {
            db.Database.EnsureDeleted();
        }

        [Fact]
        public async void UpdateSession_StudentSession_CompletelyChangeClassesAndReasons_ShouldSaveSelectedIntoDatabase()
        {
            var classes = new List<Class>()
            {
                new Class() {
                    Name = "Into to Excel",
                    ShortName = "CS 101",
                    CRN = 24668,
                    Department = new Department () {
                        Code = 134,
                        Name = "STEM"
                    }
                },
                new Class() {
                    Name = "Advanced Datastructures",
                    ShortName = "CS 387",
                    CRN = 546789,
                    Department = new Department () {
                        Code = 134,
                        Name = "STEM"
                    }
                }
            };
            var reasons = new List<Reason>()
            {
                new Reason() {
                    Id = 1,
                    Name = "Bone Use"
                },
                new Reason() {
                    Id = 2,
                    Name = "Computer Use"
                }
            };

            var person = new Person()
            {
                Email = "lbutche3@wvup.edu",
                FirstName = "Tom",
                LastName = "Betty",
                Id = 4697,
                PersonType = PersonType.Student
            };
            var semester = new Semester()
            {
                Code = 201903
            };

            var session = new Session()
            {
                InTime = DateTime.Now,
                OutTime = DateTime.Now.Add(new TimeSpan(5)),
                PersonId = person.Id,
                SemesterCode = semester.Code,
                SessionClasses = new List<SessionClass>() { new SessionClass() { ClassId = 24668 } },
                SessionReasons = new List<SessionReason>() { new SessionReason() { ReasonId = 1 } }
            };

            db.Classes.AddRange(classes);
            db.People.Add(person);
            db.Semesters.Add(semester);
            db.Sessions.Add(session);
            db.SaveChanges();

            var sessionUpdate = new SessionCreateDTO()
            {
                InTime = session.InTime,
                OutTime = session.OutTime.Value,
                PersonId = session.PersonId,
                Id = session.Id,
                SemesterCode = session.SemesterCode,
                Tutoring = session.Tutoring,
                SelectedClasses = new List<int>() { 546789 },
                SelectedReasons = new List<int>() { 2 }
            };

            await sessionController.UpdateSession(sessionUpdate.Id, sessionUpdate);

            var sessionClasses = db.SessionClasses.Where(x => x.SessionId == session.Id);
            var sessionReasons = db.SessionReasons.Where(x => x.SessionId == session.Id);
            Assert.Equal(sessionUpdate.SelectedClasses, sessionClasses.Select(x => x.ClassId).ToList());
            Assert.Equal(sessionUpdate.SelectedReasons, sessionReasons.Select(x => x.ReasonId).ToList());
        }

        [Fact]
        public async void UpdateSession_TeacherSession_SubmitWithNoClassesAndReasons_ShouldSaveSelectedIntoDatabase()
        {
            var person = new Person()
            {
                Email = "teacher@wvup.edu",
                FirstName = "Teach",
                LastName = "Pecan",
                Id = 7981,
                PersonType = PersonType.Teacher
            };
            var semester = new Semester()
            {
                Code = 201903
            };

            var session = new Session()
            {
                InTime = DateTime.Now,
                OutTime = DateTime.Now.Add(new TimeSpan(5)),
                PersonId = person.Id,
                SemesterCode = semester.Code
            };

            db.People.Add(person);
            db.Semesters.Add(semester);
            db.Sessions.Add(session);
            db.SaveChanges();

            var sessionUpdate = new SessionCreateDTO()
            {
                InTime = session.InTime,
                OutTime = session.OutTime.Value.Add(new TimeSpan(5)),
                PersonId = session.PersonId,
                Id = session.Id,
                SemesterCode = session.SemesterCode,
                Tutoring = session.Tutoring,
            };

            var result = await sessionController.UpdateSession(sessionUpdate.Id, sessionUpdate);
            var dbSession = await sessionRepo.Find(x => x.Id == sessionUpdate.Id);
            Assert.Equal(sessionUpdate.OutTime, dbSession.OutTime);
        }
    }
}
