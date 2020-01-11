using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tcs_service.EF;
using tcs_service.Models;
using tcs_service.Repos;
using tcs_service_test.Helpers;
using Xunit;

namespace tcs_service_test.Repos
{
    public class SessionsRepoTest : IDisposable
    {
        readonly SessionRepo sessionRepo;
        readonly string dbName = "SessionsRepoTest";
        readonly TCSContext db;

        public SessionsRepoTest()
        {
            var dbOptions = DbInMemory.getDbInMemoryOptions(dbName);
            db = new TCSContext(dbOptions);
            sessionRepo = new SessionRepo(dbOptions);
        }

        public void Dispose()
        {
            db.Database.EnsureDeleted();
        }

        [Fact]
        public async void Session_DeletedIsFalse_DeletedSetToTrueAfterCallingRemove()
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
                SessionReasons = new List<SessionReason>() { new SessionReason() { ReasonId = 1 } },
                Deleted = false
            };

            db.Classes.AddRange(classes);
            db.People.Add(person);
            db.Semesters.Add(semester);
            db.Sessions.Add(session);
            db.SaveChanges();

            var deletedSession = await sessionRepo.Remove(x => x.Id == session.Id);

            Assert.True(deletedSession.Deleted);
        }


        [Fact]
        public void Session_DeletedIsFalse_SessionIsReturnedByGetAll()
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
                SessionReasons = new List<SessionReason>() { new SessionReason() { ReasonId = 1 } },
                Deleted = false
            };

            db.Classes.AddRange(classes);
            db.People.Add(person);
            db.Semesters.Add(semester);
            db.Sessions.Add(session);
            db.SaveChanges();
            
            var results = sessionRepo.GetAll();

            Assert.Equal(session.OutTime, results.FirstOrDefault().OutTime);
        }

        [Fact]
        public void Session_DeletedIsTrue_SessionNotReturnedByGetAll()
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
                SessionReasons = new List<SessionReason>() { new SessionReason() { ReasonId = 1 } },
                Deleted = true
            };

            db.Classes.AddRange(classes);
            db.People.Add(person);
            db.Semesters.Add(semester);
            db.Sessions.Add(session);
            db.SaveChanges();

            var results = sessionRepo.GetAll();

            Assert.Empty(results);
        }
    }
}
