using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models.DTOs;
using tcs_service.Repos.Interfaces;
using tcs_service.Services.Interfaces;
using tcs_service.Models;

namespace tcs_service.UnitOfWorks
{
    public class UnitOfWorkPerson : IUnitOfWorkPerson
    {
        private readonly IPersonRepo personRepo;
        private readonly IScheduleRepo scheduleRepo;
        private readonly ICourseRepo courseRepo;
        private readonly ISemesterRepo semesterRepo;
        private readonly IBannerService bannerApi;

        public UnitOfWorkPerson(IPersonRepo personRepo, IScheduleRepo scheduleRepo, ICourseRepo courseRepo, ISemesterRepo semesterRepo, IBannerService bannerApi)
        {
            this.personRepo = personRepo;
            this.scheduleRepo = scheduleRepo;
            this.courseRepo = courseRepo;
            this.semesterRepo = semesterRepo;
            this.bannerApi = bannerApi;
        }

        // Need to know exactly when a new semester code should be generated
        // On what day?
        private bool IsOutdatedSemester(Semester semester, DateTime currentDate)
        {
            // 201901
            // 01 -> [January -> May]
            // 02 -> [May -> August]
            // 03 -> [August -> December]
            int getMonthCode(int month)
            {
                if (month < 5) return 1;
                if (month < 8) return 2;
                else return 3;
            };


            var year = currentDate.Year;
            var monthCode = getMonthCode(currentDate.Month);
            return semester.Code == year * 100 + monthCode;
        }

        public async Task<PersonInfoDTO> GetPersonInfo(string identifier, DateTime currentDate)
        {
            var person = await personRepo.Find(x => x.Email == identifier || x.Id.ToString() == identifier);
            var currentSemester = semesterRepo.GetAll().LastOrDefault();
            var hasSchedule = await scheduleRepo.Exist(x => currentSemester is Semester && x.SemesterCode == currentSemester.Code);

            // if outdated, call banner, and if schedule doesn't exist for current semester
            if (person is Person && IsOutdatedSemester(currentSemester, currentDate) && hasSchedule)
            {
                var schedule = scheduleRepo.GetAll(x => x.PersonId == person.Id).Select(x => x.Course);
                return new PersonInfoDTO()
                {
                    Id = person.Id,
                    Email = person.Email,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    PersonType = person.PersonType,
                    Schedule = schedule
                };
            }

            var bannerInfo = await bannerApi.GetBannerInfo(identifier);
            var newPerson = new Person()
            {
                Email = bannerInfo.Email,
                FirstName = bannerInfo.FirstName,
                LastName = bannerInfo.LastName,
                Id = bannerInfo.Id,
                PersonType = bannerInfo.Teacher ? PersonType.Teacher : PersonType.Student
            };
            var savedPerson = await personRepo.Create(newPerson);
            var semester = new Semester()
            {
                Code = bannerInfo.SemesterId
            };
            var savedSemester = await semesterRepo.Create(semester);

            // Done if person is a teacher
            if (savedPerson.PersonType == PersonType.Teacher)
            {
                return new PersonInfoDTO()
                {
                    Id = savedPerson.Id,
                    Email = savedPerson.Email,
                    FirstName = savedPerson.FirstName,
                    LastName = savedPerson.LastName,
                    PersonType = savedPerson.PersonType
                };
            }
            var courses = bannerInfo.Courses.Select(x => new Course()
            {
                CRN = x.CRN,
                Name = x.CourseName,
                ShortName = x.ShortName,
                Department = new Department()
                {
                    Code = x.Department.Code,
                    Name = x.Department.Name
                }
            });
            foreach (var c in courses)
            {
                await courseRepo.CreateOrUpdate(x => x.CRN == c.CRN, c);
                await scheduleRepo.Create(new Schedule()
                {
                    SemesterCode = bannerInfo.SemesterId,
                    PersonId = savedPerson.Id,
                    CourseCRN = c.CRN
                });
            }


            return new PersonInfoDTO()
            {
                Id = savedPerson.Id,
                Email = savedPerson.Email,
                FirstName = savedPerson.FirstName,
                LastName = savedPerson.LastName,
                PersonType = savedPerson.PersonType,
                Schedule = courses
            };
        }
    }
}