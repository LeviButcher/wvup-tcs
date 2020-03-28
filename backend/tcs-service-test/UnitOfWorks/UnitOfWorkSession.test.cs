using System;
using System.Collections.Generic;
using System.Linq;
using tcs_service.EF;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Repos;
using tcs_service_test.Helpers;
using Xunit;

namespace tcs_service_test.Controllers
{
    public class UnitOfWorkSessionTest : IDisposable
    {
        readonly string dbName = "UnitOfWorkSessionTest";
        readonly UnitOfWorkSession unitSession;
        readonly TCSContext db;

        public UnitOfWorkSessionTest()
        {
            var dbInMemory = DbInMemory.getDbInMemoryOptions(dbName);
            var personRepo = new PersonRepo(dbInMemory);
            var sessionRepo = new SessionRepo(dbInMemory);
            var reasonRepo = new ReasonRepo(dbInMemory);
            var classRepo = new ClassRepo(dbInMemory);
            db = new TCSContext(dbInMemory);
            unitSession = new UnitOfWorkSession(personRepo, reasonRepo, sessionRepo, classRepo);
        }

        public void Dispose()
        {
            db.Database.EnsureDeleted();
        }

        private int GetAmountOfRecordsThatShouldBeCreated(IEnumerable<CSVSessionUpload> sessionUploads) => sessionUploads.Select(x => x.CRNs.Count() + x.Reasons.Count() + 1).Sum();

        private bool CheckSessionWhereCreate(IEnumerable<CSVSessionUpload> sessionUploads)
        {
            return sessionUploads.All(s =>
               db.Sessions.Any(x => x.Person.Email == s.Email &&
               x.InTime == s.InTime &&
               x.OutTime == s.OutTime &&
               x.Tutoring == s.Tutoring &&
               x.SessionClasses.All(a => s.CRNs.Contains(a.ClassId)) &&
               s.Reasons.All(r => x.SessionReasons.Any(sre => sre.Reason.Name == r))
           ));
        }

        List<Person> personList = new List<Person>() {
                new Person () {
                Email = "lbutche3@wvup.edu",
                FirstName = "Levi",
                LastName = "Greatest",
                Id = 13246,
                PersonType = PersonType.Student
                },
                new Person() {
                    Email = "srickard3@wvup.edu",
                    FirstName = "Sean",
                    LastName = "Epic",
                    Id = 79468
                }
            };
        Semester semester = new Semester()
        {
            Code = 201901
        };
        List<Class> courseList = new List<Class>() {
                new Class () {
                    CRN = 78945,
                    Name = "Intro to Comp Science",
                    ShortName = "CS 101",
                    Department = new Department () {
                        Code = 4678,
                        Name = "STEM"
                    }
                },
                new Class() {
                    CRN = 46546,
                    Name = "English 101",
                    ShortName = "EN101",
                    Department = new Department() {
                        Code = 7564,
                        Name = "Literature"
                    },
                }
            };
        List<Reason> reasons = new List<Reason>() {
                new Reason () {
                Name = "Computer Use"
                },
                new Reason () {
                Name = "Bone Use"
                }
            };


        [Fact]
        public async void UploadSessions_AllPeopleCoursesAndReasonsExist_ShouldBeSuccessful()
        {
            db.People.AddRange(personList);
            db.Semesters.Add(semester);
            db.Classes.AddRange(courseList);
            db.Reasons.AddRange(reasons);
            await db.SaveChangesAsync();


            var sessionUploads = new List<CSVSessionUpload>() {
                new CSVSessionUpload() {
                    Email = "lbutche3@wvup.edu",
                    InTime = DateTime.Now,
                    OutTime = DateTime.Now.Add(TimeSpan.FromHours(2)),
                    CRNs = new List<int>(){78945,},
                    Reasons = new List<string>(){"Computer Use", "Bone Use"},
                    Tutoring = true
                },
                new CSVSessionUpload() {
                    Email = "srickard3@wvup.edu",
                    InTime = DateTime.Now,
                    OutTime = DateTime.Now.Add(TimeSpan.FromHours(5)),
                    CRNs = new List<int>(){78945, 46546},
                    Reasons = new List<string>(){"Computer Use"},
                    Tutoring = false
                }
            };
            var res = await unitSession.UploadSessions(sessionUploads);
            Assert.Equal(GetAmountOfRecordsThatShouldBeCreated(sessionUploads), res);
            Assert.True(CheckSessionWhereCreate(sessionUploads));
        }

        [Fact]
        public async void UploadSessions_PersonDoesNotExist_ShouldThrowError()
        {
            var semester = new Semester()
            {
                Code = 201901
            };
            var courseList = new List<Class>() {
                new Class () {
                    CRN = 78945,
                    Name = "Intro to Comp Science",
                    ShortName = "CS 101",
                    Department = new Department () {
                        Code = 4678,
                        Name = "STEM"
                    }
                },
                new Class() {
                    CRN = 46546,
                    Name = "English 101",
                    ShortName = "EN101",
                    Department = new Department() {
                        Code = 7564,
                        Name = "Literature"
                    },
                }
            };
            var reasons = new List<Reason>() {
                new Reason () {
                Name = "Computer Use"
                },
                new Reason () {
                Name = "Bone Use"
                }
            };
            db.Semesters.Add(semester);
            db.Classes.AddRange(courseList);
            db.Reasons.AddRange(reasons);
            await db.SaveChangesAsync();

            var sessionUploads = new List<CSVSessionUpload>() {
                new CSVSessionUpload() {
                    Email = "lbutche3@wvup.edu",
                    InTime = DateTime.Now,
                    OutTime = DateTime.Now.Add(TimeSpan.FromHours(2)),
                    CRNs = new List<int>(){78945,},
                    Reasons = new List<string>(){"Computer Use", "Bone Use"},
                    Tutoring = true
                }
            };

            var err = await Assert.ThrowsAsync<TCSException>(async () =>
                await unitSession.UploadSessions(sessionUploads));
            Assert.Contains("lbutche3@wvup.edu does not exist", err.Message);
            Assert.False(CheckSessionWhereCreate(sessionUploads));
        }

        [Fact]
        public async void UploadSessions_ClassDoesNotExist_ShouldThrowError()
        {
            db.People.AddRange(personList);
            db.Semesters.Add(semester);
            db.Reasons.AddRange(reasons);
            await db.SaveChangesAsync();

            var sessionUploads = new List<CSVSessionUpload>() {
                new CSVSessionUpload() {
                    Email = "lbutche3@wvup.edu",
                    InTime = DateTime.Now,
                    OutTime = DateTime.Now.Add(TimeSpan.FromHours(2)),
                    CRNs = new List<int>(){78945,},
                    Reasons = new List<string>(){"Computer Use", "Bone Use"},
                    Tutoring = true
                }
            };

            var err = await Assert.ThrowsAsync<TCSException>(async () =>
                await unitSession.UploadSessions(sessionUploads));
            Assert.Contains("Class with CRN: 78945 does not exist", err.Message);
            Assert.False(CheckSessionWhereCreate(sessionUploads));
        }

        [Fact]
        public async void UploadSessions_ReasonDoesNotExist_ShouldThrowError()
        {
            db.People.AddRange(personList);
            db.Semesters.Add(semester);
            db.Classes.AddRange(courseList);
            await db.SaveChangesAsync();

            var sessionUploads = new List<CSVSessionUpload>() {
                new CSVSessionUpload() {
                    Email = "lbutche3@wvup.edu",
                    InTime = DateTime.Now,
                    OutTime = DateTime.Now.Add(TimeSpan.FromHours(2)),
                    CRNs = new List<int>(){78945,},
                    Reasons = new List<string>(){"Computer Use"},
                    Tutoring = true
                }
            };

            var err = await Assert.ThrowsAsync<TCSException>(async () =>
                await unitSession.UploadSessions(sessionUploads));
            Assert.Contains("Reason with Name: 'Computer Use' does not exist", err.Message);
            Assert.False(CheckSessionWhereCreate(sessionUploads));
        }

        [Fact]
        public async void UploadSessions_SeveralThingsDoNotExist_ShouldThrowError()
        {
            var sessionUploads = new List<CSVSessionUpload>() {
                new CSVSessionUpload() {
                    Email = "lbutche3@wvup.edu",
                    InTime = DateTime.Now,
                    OutTime = DateTime.Now.Add(TimeSpan.FromHours(2)),
                    CRNs = new List<int>(){78945,},
                    Reasons = new List<string>(){"Computer Use"},
                    Tutoring = true
                }
            };

            var err = await Assert.ThrowsAsync<TCSException>(async () =>
                await unitSession.UploadSessions(sessionUploads));
            Assert.Contains("lbutche3@wvup.edu does not exist", err.Message);
            Assert.Contains("Reason with Name: 'Computer Use' does not exist", err.Message);
            Assert.Contains("Class with CRN: 78945 does not exist", err.Message);
            Assert.False(CheckSessionWhereCreate(sessionUploads));
        }
    }
}