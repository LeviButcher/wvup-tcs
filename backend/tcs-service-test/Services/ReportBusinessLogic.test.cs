using System;
using System.Collections.Generic;
using System.Linq;
using tcs_service.Models;
using tcs_service.Models.DTO;
using tcs_service.Services;
using Xunit;

namespace tcs_service_test.Services
{
    public class ReportBusinessLogicTest
    {
        public ReportBusinessLogicTest() { } 

        [Fact]
        public void WeelyVisits_SessionOnStartDay_ResultCountShouldBeOne()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 24, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 24, 11, 00, 0),
                }
            };

            var results = ReportsBusinessLogic.WeeklyVisits(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(1, results.FirstOrDefault().Count);
        }

        [Fact]
        public void WeelyVisits_SessionOnEndDay_ResultCountShouldBeOne()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 30, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 30, 11, 00, 0),
                }
            };

            var results = ReportsBusinessLogic.WeeklyVisits(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(1, results.FirstOrDefault().Count);
        }

        [Fact]
        public void WeelyVisits_SessionOnTheDayBeforeStartDate_ResultCountShouldBeZero()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 23, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 23, 11, 00, 0),
                }
            };

            var results = ReportsBusinessLogic.WeeklyVisits(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(0, results.FirstOrDefault().Count);
        }

        [Fact]
        public void WeelyVisits_SessionOnTheDayAfterEndDate_ResultCountShouldBeZero()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 31, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 31, 11, 00, 0),
                }
            };

            var results = ReportsBusinessLogic.WeeklyVisits(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(0, results.FirstOrDefault().Count);
        }

        [Fact]
        public void WeelyVisits_TwoWeekSpan_OneSessionEachWeek_ResultsShouldHaveCountOfTwo_EachWeekShouldHaveCountOfOne()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 01, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 01, 11, 00, 0),
                },
                new Session() {
                    InTime = new DateTime(2020, 12, 10, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 10, 11, 00, 0),
                },
            };

            var results = ReportsBusinessLogic.WeeklyVisits(sessions, new DateTime(2020, 12, 1), new DateTime(2020, 12, 14));
            Assert.Equal(2, results.Count());
            Assert.Equal(1, results[0].Count);
            Assert.Equal(1, results[1].Count);
        }


        [Fact]
        public void PeakHours_SessionOnStartDayAt10AM_ResultCountShouldBeOne_HourShouldBe10AM()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 01, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 01, 11, 00, 0),
                }
            };

            var results = ReportsBusinessLogic.PeakHours(sessions, new DateTime(2020, 12, 01), new DateTime(2020, 12, 07));
            Assert.Single(results);
            Assert.Equal(1, results.Where(x => x.Hour == "10 A.M").FirstOrDefault().Count);
        }

        [Fact]
        public void PeakHours_SessionOnEndDayAt10AM_ResultListShouldHaveOneElement_HourShouldBe10AM()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 07, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 07, 11, 00, 0),
                }
            };

            var results = ReportsBusinessLogic.PeakHours(sessions, new DateTime(2020, 12, 01), new DateTime(2020, 12, 07));
            Assert.Single(results);
            Assert.Equal(1, results.Where(x => x.Hour == "10 A.M").FirstOrDefault().Count);
        }

        [Fact]
        public void PeakHours_SessionsWithinStartAndEndDate_2SessionsAt10AM_OneSessionAt5PM_ResultCountShouldBeTwo_10AMCountShouldBeTwo_5PMCountShouldBeOne()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 03, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 03, 11, 00, 0),
                },
                new Session() {
                    InTime = new DateTime(2020, 12, 05, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 05, 11, 00, 0),    
                },
                new Session() {
                    InTime = new DateTime(2020, 12, 06, 17, 0, 0),
                    OutTime = new DateTime(2020, 12, 06, 17, 00, 0),
                }
            };

            var results = ReportsBusinessLogic.PeakHours(sessions, new DateTime(2020, 12, 01), new DateTime(2020, 12, 07));
            Assert.Equal(2, results.Count);
            Assert.Equal(2, results.Where(x => x.Hour == "10 A.M").FirstOrDefault().Count);
            Assert.Equal(1, results.Where(x => x.Hour == "5 P.M").FirstOrDefault().Count);
        }

        [Fact]
        public void ClassTours_TourOnStartDay_ResultListShouldHaveOneElement()
        {
            var tours = new List<ClassTour>() {
                new ClassTour() {
                    DayVisited = new DateTime(2020, 12, 01 ),
                    Name = "Point Pleasant High School",
                    NumberOfStudents = 23
                }
            };
            
            var results = ReportsBusinessLogic.ClassTours(tours, new DateTime(2020, 12, 01), new DateTime(2020, 12, 07));
            Assert.Single(results);
        }

        [Fact]
        public void ClassTours_TourOnEndDay_ResultListShouldHaveOneElement()
        {
            var tours = new List<ClassTour>() {
                new ClassTour() {
                    DayVisited = new DateTime(2020, 12, 07),
                    Name = "Point Pleasant High School",
                    NumberOfStudents = 23
                }
            };

            var results = ReportsBusinessLogic.ClassTours(tours, new DateTime(2020, 12, 01), new DateTime(2020, 12, 07));
            Assert.Single(results);
        }

        [Fact]
        public void ClassTours_SchoolVisitsTwoTimesWithinDates_ResultListShouldHaveOneElement_StudentsShouldBeSumOfNumberOfStudentsSForBothVisits()
        {
            var tours = new List<ClassTour>() {
                new ClassTour() {
                    DayVisited = new DateTime(2020, 12, 01),
                    Name = "Point Pleasant High School",
                    NumberOfStudents = 25
                },
                new ClassTour() {
                    DayVisited = new DateTime(2020, 12, 07),
                    Name = "Point Pleasant High School",
                    NumberOfStudents = 25
                }
            };

            var results = ReportsBusinessLogic.ClassTours(tours, new DateTime(2020, 12, 01), new DateTime(2020, 12, 07));
            Assert.Single(results);
            Assert.Equal(50, results.Where(x => x.Name == "Point Pleasant High School").FirstOrDefault().Students);
        }

        [Fact]
        public void ClassTours_SchoolVisitsTwoTimesWithinDates_OtherSchoolVisitsOnce_FirstSchoolNumberOfStudentsShouldBeSummed_SecondSchoolStudentCountShouldBeEqualToNumberOfStudentsForThatTour()
        {
            var tours = new List<ClassTour>() {
                new ClassTour() {
                    DayVisited = new DateTime(2020, 12, 01),
                    Name = "Point Pleasant High School",
                    NumberOfStudents = 25
                },
                new ClassTour() {
                    DayVisited = new DateTime(2020, 12, 07),
                    Name = "Point Pleasant High School",
                    NumberOfStudents = 25
                },
                new ClassTour() {
                    DayVisited = new DateTime(2020, 12, 06),
                    Name = "Parkersburg High School",
                    NumberOfStudents = 11
                },
            };

            var results = ReportsBusinessLogic.ClassTours(tours, new DateTime(2020, 12, 01), new DateTime(2020, 12, 07));
            Assert.Equal(2, results.Count());
            Assert.Equal(50, results.Where(x => x.Name == "Point Pleasant High School").FirstOrDefault().Students);
            Assert.Equal(11, results.Where(x => x.Name == "Parkersburg High School").FirstOrDefault().Students);
        }

        [Fact]
        public void Volunteers_SessionOnStartDay_ResultListShouldHaveOneElement()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 01, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 01, 11, 00, 0),
                    Person = new Person()
                    {
                        FirstName = "Teacher",
                        LastName = "Teach",
                        Email = "teacher@wvup.edu",
                        PersonType = PersonType.Teacher
                    }
                }
            };

            var results = ReportsBusinessLogic.Volunteers(sessions, new DateTime(2020, 12, 01), new DateTime(2020, 12, 07));
            Assert.Single(results);
        }

        [Fact]
        public void Volunteers_SessionOnEndDay_ResultListShouldHaveOneElement()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 07, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 07, 11, 00, 0),
                    Person = new Person()
                    {
                        FirstName = "Teacher",
                        LastName = "Teach",
                        Email = "teacher@wvup.edu",
                        PersonType = PersonType.Teacher
                    }
                }
            };

            var results = ReportsBusinessLogic.Volunteers(sessions, new DateTime(2020, 12, 01), new DateTime(2020, 12, 07));
            Assert.Single(results);
        }


        [Fact]
        public void Volunteers_TeacherHasTwoOneHourSessions_VolunteerHoursShouldBeTwoHours()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 05, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 05, 11, 00, 0),
                    Person = new Person()
                    {
                        FirstName = "Teacher",
                        LastName = "Teach",
                        Email = "teacher@wvup.edu",
                        PersonType = PersonType.Teacher
                    }
                },
                new Session() {
                    InTime = new DateTime(2020, 12, 07, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 07, 11, 00, 0),
                    Person = new Person()
                    {
                        FirstName = "Teacher",
                        LastName = "Teach",
                        Email = "teacher@wvup.edu",
                        PersonType = PersonType.Teacher
                    }
                },
            };

            var results = ReportsBusinessLogic.Volunteers(sessions, new DateTime(2020, 12, 01), new DateTime(2020, 12, 07));
            Assert.Equal(2, results.Where(x => x.TeacherEmail == "teacher@wvup.edu").FirstOrDefault().TotalHours);
        }

        [Fact]
        public void Volunteers_TwoTeachersHaveOneHourSessions_EachTeachersHoursSummedCorrectly()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 05, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 05, 11, 00, 0),
                    Person = new Person()
                    {
                        FirstName = "Teacher",
                        LastName = "Teach",
                        Email = "teacher@wvup.edu",
                        PersonType = PersonType.Teacher
                    }
                },
                new Session() {
                    InTime = new DateTime(2020, 12, 07, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 07, 11, 00, 0),
                    Person = new Person()
                    {
                        FirstName = "Teacher",
                        LastName = "Teach",
                        Email = "otherteacher@wvup.edu",
                        PersonType = PersonType.Teacher
                    }
                },
            };

            var results = ReportsBusinessLogic.Volunteers(sessions, new DateTime(2020, 12, 01), new DateTime(2020, 12, 07));
            Assert.Equal(1, results.Where(x => x.TeacherEmail == "teacher@wvup.edu").FirstOrDefault().TotalHours);
            Assert.Equal(1, results.Where(x => x.TeacherEmail == "otherteacher@wvup.edu").FirstOrDefault().TotalHours);
        }

        [Fact]
        public void Reasons_SessionOnStartDay_ResultCountShouldBeOne()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 24, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 24, 11, 00, 0),
                    Tutoring = false,
                    Person = new Person() { PersonType = PersonType.Student, Email = "student1@wvup.edu", FirstName = "Student", LastName = "One", Id = 12345 },
                    SessionClasses = new List<SessionClass>()
                    {
                        new SessionClass() { Class = new Class(){ Name = "Art 101", CRN = 123, ShortName = "Art", DepartmentCode = 111 } }
                    },
                    SessionReasons = new List<SessionReason>()
                    {
                        new SessionReason() { Reason = new Reason() { Name = "Study Time", Deleted = false } }
                    }
                }
            };

            var results = ReportsBusinessLogic.Reasons(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Single(results);
        }

        [Fact]
        public void Reasons_SessionOnEndDay_ResultCountShouldBeOne()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 30, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 30, 11, 00, 0),
                    Tutoring = false,
                    Person = new Person() { PersonType = PersonType.Student, Email = "student1@wvup.edu", FirstName = "Student", LastName = "One", Id = 12345 },
                    SessionClasses = new List<SessionClass>()
                    {
                        new SessionClass() { Class = new Class(){ Name = "Art 101", CRN = 123, ShortName = "Art", DepartmentCode = 111 } }
                    },
                    SessionReasons = new List<SessionReason>()
                    {
                        new SessionReason() { Reason = new Reason() { Name = "Study Time", Deleted = false } }
                    }
                }
            };

            var results = ReportsBusinessLogic.Reasons(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Single(results);
        }

        [Fact]
        public void Reasons_SessionWithClassSelectedAndNoReasonSelected_TutoringIsTrue_VisitsForTutoringShouldBeOne()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 30, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 30, 11, 00, 0),
                    Tutoring = true,
                    Person = new Person() { PersonType = PersonType.Student, Email = "student1@wvup.edu", FirstName = "Student", LastName = "One", Id = 12345 },
                    SessionClasses = new List<SessionClass>()
                    {
                        new SessionClass() { Class = new Class(){ Name = "Art 101", CRN = 123, ShortName = "Art", DepartmentCode = 111 } }
                    }                    
                }
            };

            var results = ReportsBusinessLogic.Reasons(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(1, results.Where(x => x.ReasonName == "Tutoring").FirstOrDefault().Visits);
        }

        [Fact]
        public void Reasons_SessionWithClassSelectedAndStudyTimeSelected_TutoringIsTrue_VisitsForTutoringShouldBeOne_VisitsForStudyTimeShouldBeOne()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 30, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 30, 11, 00, 0),
                    Tutoring = true,
                    Person = new Person() { PersonType = PersonType.Student, Email = "student1@wvup.edu", FirstName = "Student", LastName = "One", Id = 12345 },
                    SessionClasses = new List<SessionClass>()
                    {
                        new SessionClass() { Class = new Class(){ Name = "Art 101", CRN = 123, ShortName = "Art", DepartmentCode = 111 } }
                    },
                    SessionReasons = new List<SessionReason>()
                    {
                        new SessionReason() { Reason = new Reason() { Name = "Study Time", Deleted = false } }
                    }
                }
            };

            var results = ReportsBusinessLogic.Reasons(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(1, results.Where(x => x.ReasonName == "Tutoring").FirstOrDefault().Visits);
            Assert.Equal(1, results.Where(x => x.ReasonName == "Study Time").FirstOrDefault().Visits);
        }

        [Fact]
        public void Reasons_SessionWithClassSelectedAndStudyTimeSelected_TutoringIsFalse_VisitsForStudyTimeShouldBeOne()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 30, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 30, 11, 00, 0),
                    Tutoring = false,
                    Person = new Person() { PersonType = PersonType.Student, Email = "student1@wvup.edu", FirstName = "Student", LastName = "One", Id = 12345 },
                    SessionClasses = new List<SessionClass>()
                    {
                        new SessionClass() { Class = new Class(){ Name = "Art 101", CRN = 123, ShortName = "Art", DepartmentCode = 111 } }
                    },
                    SessionReasons = new List<SessionReason>()
                    {
                        new SessionReason() { Reason = new Reason() { Name = "Study Time", Deleted = false } }
                    }
                }
            };

            var results = ReportsBusinessLogic.Reasons(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(1, results.Where(x => x.ReasonName == "Study Time").FirstOrDefault().Visits);
        }

        [Fact]
        public void Reasons_SessionWithTutoringTrue_TutoringVisitCountIsOne()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 24, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 24, 11, 00, 0),
                    Tutoring = true,
                    Person = new Person() { PersonType = PersonType.Student, Email = "student1@wvup.edu", FirstName = "Student", LastName = "One", Id = 12345 },
                    SessionClasses = new List<SessionClass>()
                    {
                        new SessionClass() { Class = new Class(){ Name = "Art 101", CRN = 123, ShortName = "Art", DepartmentCode = 111 } }
                    }
                }
            };

            var results = ReportsBusinessLogic.Reasons(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(1, results.Where(x => x.ReasonName == "Tutoring").FirstOrDefault().Visits);
        }
        
        [Fact]
        public void Reasons_TwoSessionsForAClass_TutoringIsFalse_TwoDifferentReasonsForVisit_ResultsIncludesClassTwice_OnceForEachReason()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 24, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 24, 11, 00, 0),
                    Tutoring = false,
                    Person = new Person() { PersonType = PersonType.Student, Email = "student1@wvup.edu", FirstName = "Student", LastName = "One", Id = 12345 },
                    SessionClasses = new List<SessionClass>()
                    {
                        new SessionClass() { Class = new Class(){ Name = "Art 101", CRN = 123, ShortName = "Art", DepartmentCode = 111 } }
                    },
                    SessionReasons = new List<SessionReason>()
                    {
                        new SessionReason() { Reason = new Reason() { Name = "Study Time", Deleted = false } }
                    }
                },
                new Session() {
                    InTime = new DateTime(2020, 12, 24, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 24, 11, 00, 0),
                    Tutoring = false,
                    Person = new Person() { PersonType = PersonType.Student, Email = "student1@wvup.edu", FirstName = "Student", LastName = "One", Id = 12345 },
                    SessionClasses = new List<SessionClass>()
                    {
                        new SessionClass() { Class = new Class(){ Name = "Art 101", CRN = 123, ShortName = "Art", DepartmentCode = 111 } }
                    },
                    SessionReasons = new List<SessionReason>()
                    {
                        new SessionReason() { Reason = new Reason() { Name = "Printer Use", Deleted = false } }
                    }
                },

            };

            var results = ReportsBusinessLogic.Reasons(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(2, results.Where(x => x.ClassName == "Art 101").Count());
        }

        [Fact]
        public void Reasons_TwoSessionsForAClass_TutoringIsTrue_TwoDifferentReasonsForVisit_ResultsIncludesClassFourTimes_OnceForEachReason()
        {
            var sessions = new List<Session>() {
                new Session() {
                    InTime = new DateTime(2020, 12, 24, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 24, 11, 00, 0),
                    Tutoring = true,
                    Person = new Person() { PersonType = PersonType.Student, Email = "student1@wvup.edu", FirstName = "Student", LastName = "One", Id = 12345 },
                    SessionClasses = new List<SessionClass>()
                    {
                        new SessionClass() { Class = new Class(){ Name = "Art 101", CRN = 123, ShortName = "Art", DepartmentCode = 111 } }
                    },
                    SessionReasons = new List<SessionReason>()
                    {
                        new SessionReason() { Reason = new Reason() { Name = "Study Time", Deleted = false } }
                    }
                },
                new Session() {
                    InTime = new DateTime(2020, 12, 24, 10, 0, 0),
                    OutTime = new DateTime(2020, 12, 24, 11, 00, 0),
                    Tutoring = true,
                    Person = new Person() { PersonType = PersonType.Student, Email = "student1@wvup.edu", FirstName = "Student", LastName = "One", Id = 12345 },
                    SessionClasses = new List<SessionClass>()
                    {
                        new SessionClass() { Class = new Class(){ Name = "Art 101", CRN = 123, ShortName = "Art", DepartmentCode = 111 } }
                    },
                    SessionReasons = new List<SessionReason>()
                    {
                        new SessionReason() { Reason = new Reason() { Name = "Printer Use", Deleted = false } }
                    }
                },

            };

            var results = ReportsBusinessLogic.Reasons(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(3, results.Where(x => x.ClassName == "Art 101").Count());
            Assert.Equal(2, results.Where(x => x.ReasonName == "Tutoring").FirstOrDefault().Visits);
        }

        [Fact]
        public void SuccessReport_OneClass_StudentReceivedA_ClassPassedSuccessfullyIsOne_ClassCompletedSuccessfullyIsOne_DroppedStudentCountIsZero_UniqueCountIsOne()
        {
            var classWithGradeList = new List<ClassWithGradeDTO>() {
                new ClassWithGradeDTO()
                {
                    CourseName = "History",
                    CRN = 111,
                    DepartmentName = "History Dept",
                    Grade = Grade.A
                }
            };

            var results = ReportsBusinessLogic.SuccessReport(classWithGradeList);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().CompletedCourseCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().PassedSuccessfullyCount);
            Assert.Equal(0, results.Where(x => x.CRN == 111).FirstOrDefault().DroppedStudentCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().UniqueStudentCount);
        }

        [Fact]
        public void SuccessReport_OneClass_StudentReceivedB_ClassPassedSuccessfullyIsOne_ClassCompletedSuccessfullyIsOne_DroppedStudentCountIsZero_UniqueCountIsOne()
        {
            var classWithGradeList = new List<ClassWithGradeDTO>() {
                new ClassWithGradeDTO()
                {
                    CourseName = "History",
                    CRN = 111,
                    DepartmentName = "History Dept",
                    Grade = Grade.B
                }
            };

            var results = ReportsBusinessLogic.SuccessReport(classWithGradeList);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().CompletedCourseCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().PassedSuccessfullyCount);
            Assert.Equal(0, results.Where(x => x.CRN == 111).FirstOrDefault().DroppedStudentCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().UniqueStudentCount);
        }

        [Fact]
        public void SuccessReport_OneClass_StudentReceivedC_ClassPassedSuccessfullyIsOne_ClassCompletedSuccessfullyIsOne_DroppedStudentCountIsZero_UniqueCountIsOne()
        {
            var classWithGradeList = new List<ClassWithGradeDTO>() {
                new ClassWithGradeDTO()
                {
                    CourseName = "History",
                    CRN = 111,
                    DepartmentName = "History Dept",
                    Grade = Grade.C
                }
            };

            var results = ReportsBusinessLogic.SuccessReport(classWithGradeList);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().CompletedCourseCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().PassedSuccessfullyCount);
            Assert.Equal(0, results.Where(x => x.CRN == 111).FirstOrDefault().DroppedStudentCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().UniqueStudentCount);
        }

        [Fact]
        public void SuccessReport_OneClass_StudentReceivedD_ClassPassedSuccessfullyIsZero_ClassCompletedSuccessfullyIsOne_DroppedStudentCountIsZero_UniqueCountIsOne()
        {
            var classWithGradeList = new List<ClassWithGradeDTO>() {
                new ClassWithGradeDTO()
                {
                    CourseName = "History",
                    CRN = 111,
                    DepartmentName = "History Dept",
                    Grade = Grade.D
                }
            };

            var results = ReportsBusinessLogic.SuccessReport(classWithGradeList);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().CompletedCourseCount);
            Assert.Equal(0, results.Where(x => x.CRN == 111).FirstOrDefault().PassedSuccessfullyCount);
            Assert.Equal(0, results.Where(x => x.CRN == 111).FirstOrDefault().DroppedStudentCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().UniqueStudentCount);
        }

        [Fact]
        public void SuccessReport_OneClass_StudentReceivedF_ClassPassedSuccessfullyIsZero_ClassCompletedSuccessfullyIsOne_DroppedStudentCountIsZero_UniqueCountIsOne()
        {
            var classWithGradeList = new List<ClassWithGradeDTO>() {
                new ClassWithGradeDTO()
                {
                    CourseName = "History",
                    CRN = 111,
                    DepartmentName = "History Dept",
                    Grade = Grade.F
                }
            };

            var results = ReportsBusinessLogic.SuccessReport(classWithGradeList);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().CompletedCourseCount);
            Assert.Equal(0, results.Where(x => x.CRN == 111).FirstOrDefault().PassedSuccessfullyCount);
            Assert.Equal(0, results.Where(x => x.CRN == 111).FirstOrDefault().DroppedStudentCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().UniqueStudentCount);
        }

        [Fact]
        public void SuccessReport_OneClass_StudentReceivedFIW_ClassPassedSuccessfullyIsZero_ClassCompletedSuccessfullyIsZero_DroppedStudentCountIsOne_UniqueCountIsOne()
        {
            var classWithGradeList = new List<ClassWithGradeDTO>() {
                new ClassWithGradeDTO()
                {
                    CourseName = "History",
                    CRN = 111,
                    DepartmentName = "History Dept",
                    Grade = Grade.FIW
                }
            };

            var results = ReportsBusinessLogic.SuccessReport(classWithGradeList);
            Assert.Equal(0, results.Where(x => x.CRN == 111).FirstOrDefault().CompletedCourseCount);
            Assert.Equal(0, results.Where(x => x.CRN == 111).FirstOrDefault().PassedSuccessfullyCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().DroppedStudentCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().UniqueStudentCount);
        }

        [Fact]
        public void SuccessReport_OneClass_StudentReceivedW_ClassPassedSuccessfullyIsZero_ClassCompletedSuccessfullyIsZero_DroppedStudentCountIsOne_UniqueCountIsOne()
        {
            var classWithGradeList = new List<ClassWithGradeDTO>() {
                new ClassWithGradeDTO()
                {
                    CourseName = "History",
                    CRN = 111,
                    DepartmentName = "History Dept",
                    Grade = Grade.W
                }
            };

            var results = ReportsBusinessLogic.SuccessReport(classWithGradeList);
            Assert.Equal(0, results.Where(x => x.CRN == 111).FirstOrDefault().CompletedCourseCount);
            Assert.Equal(0, results.Where(x => x.CRN == 111).FirstOrDefault().PassedSuccessfullyCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().DroppedStudentCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().UniqueStudentCount);
        }

        [Fact]
        public void SuccessReport_OneClass_StudentReceivedI_ClassPassedSuccessfullyIsOne_ClassCompletedSuccessfullyIsOne_DroppedStudentCountIsZero_UniqueCountIsOne()
        {
            var classWithGradeList = new List<ClassWithGradeDTO>() {
                new ClassWithGradeDTO()
                {
                    CourseName = "History",
                    CRN = 111,
                    DepartmentName = "History Dept",
                    Grade = Grade.I
                }
            };

            var results = ReportsBusinessLogic.SuccessReport(classWithGradeList);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().CompletedCourseCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().PassedSuccessfullyCount);
            Assert.Equal(0, results.Where(x => x.CRN == 111).FirstOrDefault().DroppedStudentCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().UniqueStudentCount);
        }

        [Fact]
        public void SuccessReport_OneClass_TwoStudents_OneReceivesA_OneReceivesF_CompletedCourseCountIsTwo_PassedSuccessfullyCountIsOne_DroppedCountIsZero_UniqueStudentsIsTwo()
        {
            var classWithGradeList = new List<ClassWithGradeDTO>() {
                new ClassWithGradeDTO()
                {
                    CourseName = "History",
                    CRN = 111,
                    DepartmentName = "History Dept",
                    Grade = Grade.A
                },
                new ClassWithGradeDTO()
                {
                    CourseName = "History",
                    CRN = 111,
                    DepartmentName = "History Dept",
                    Grade = Grade.F
                },
            };

            var results = ReportsBusinessLogic.SuccessReport(classWithGradeList);
            Assert.Equal(2, results.Where(x => x.CRN == 111).FirstOrDefault().CompletedCourseCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().PassedSuccessfullyCount);
            Assert.Equal(0, results.Where(x => x.CRN == 111).FirstOrDefault().DroppedStudentCount);
            Assert.Equal(2, results.Where(x => x.CRN == 111).FirstOrDefault().UniqueStudentCount);
        }

        [Fact]
        public void SuccessReport_TwoClasses_TwoStudents_BothReceiveA_CompletedCourseCountIsTwo_EachClassCompletedAndPassedCountIsOne_DroppedIsZero_UniqueIsOne()
        {
            var classWithGradeList = new List<ClassWithGradeDTO>() {
                new ClassWithGradeDTO()
                {
                    CourseName = "History",
                    CRN = 111,
                    DepartmentName = "History Dept",
                    Grade = Grade.A
                },
                new ClassWithGradeDTO()
                {
                    CourseName = "English",
                    CRN = 123,
                    DepartmentName = "English Dept",
                    Grade = Grade.A
                },
            };

            var results = ReportsBusinessLogic.SuccessReport(classWithGradeList);
            Assert.Equal(2, results.Count);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().CompletedCourseCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().PassedSuccessfullyCount);
            Assert.Equal(0, results.Where(x => x.CRN == 111).FirstOrDefault().DroppedStudentCount);
            Assert.Equal(1, results.Where(x => x.CRN == 111).FirstOrDefault().UniqueStudentCount);
            Assert.Equal(1, results.Where(x => x.CRN == 123).FirstOrDefault().CompletedCourseCount);
            Assert.Equal(1, results.Where(x => x.CRN == 123).FirstOrDefault().PassedSuccessfullyCount);
            Assert.Equal(0, results.Where(x => x.CRN == 123).FirstOrDefault().DroppedStudentCount);
            Assert.Equal(1, results.Where(x => x.CRN == 123).FirstOrDefault().UniqueStudentCount);
        }
    }
}
