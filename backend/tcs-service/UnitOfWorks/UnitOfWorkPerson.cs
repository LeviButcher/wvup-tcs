using System;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models.DTOs;
using tcs_service.Repos.Interfaces;
using tcs_service.Services.Interfaces;
using tcs_service.Models;
using tcs_service.UnitOfWorks.Interfaces;

namespace tcs_service.UnitOfWorks
{
    public class UnitOfWorkPerson : IUnitOfWorkPerson
    {
        private readonly IPersonRepo personRepo;
        private readonly IScheduleRepo scheduleRepo;
        private readonly IClassRepo classRepo;
        private readonly ISemesterRepo semesterRepo;
        private readonly IDepartmentRepo departmentRepo;
        private readonly IBannerService bannerApi;

        public UnitOfWorkPerson(IPersonRepo personRepo, IScheduleRepo scheduleRepo, IClassRepo classRepo, ISemesterRepo semesterRepo, IDepartmentRepo departmentRepo, IBannerService bannerApi)
        {
            this.personRepo = personRepo;
            this.scheduleRepo = scheduleRepo;
            this.classRepo = classRepo;
            this.semesterRepo = semesterRepo;
            this.departmentRepo = departmentRepo;
            this.bannerApi = bannerApi;
        }

        public async Task<PersonInfoDTO> GetPersonInfo(string identifier)
        {
            var person = await personRepo.Find(x => x.Email == identifier || x.Id.ToString() == identifier);

            var bannerInfo = await bannerApi.GetBannerInfo(identifier);
            var hasSchedule = await scheduleRepo.Exist(x => person is Person && x.SemesterCode == bannerInfo.SemesterId && x.PersonId == person.Id);

            // if person has a schedule, just return their schedule
            if (person is Person && hasSchedule)
            {
                var schedule = scheduleRepo.GetAll(x => x.PersonId == person.Id).Select(x => x.Class);
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


            var newPerson = new Person()
            {
                Email = bannerInfo.Email,
                FirstName = bannerInfo.FirstName,
                LastName = bannerInfo.LastName,
                Id = bannerInfo.Id,
                PersonType = bannerInfo.Teacher ? PersonType.Teacher : PersonType.Student
            };
            var savedPerson = await personRepo.CreateOrUpdate(x => x.Id == newPerson.Id, newPerson);
            var semester = new Semester()
            {
                Code = bannerInfo.SemesterId
            };
            var savedSemester = await semesterRepo.CreateOrUpdate(x => x.Code == semester.Code, semester);

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
            var departments = bannerInfo.Classes.Select(x => x.Department).Select(x => new Department()
            {
                Code = x.Code,
                Name = x.Name
            });
            foreach (var d in departments)
            {
                await departmentRepo.CreateOrUpdate(x => x.Code == d.Code, d);
            }

            var courses = bannerInfo.Classes.Select(x => new Class()
            {
                CRN = x.CRN,
                Name = x.CourseName,
                ShortName = x.ShortName,
                DepartmentCode = x.Department.Code
            });

            foreach (var c in courses)
            {
                await classRepo.CreateOrUpdate(x => x.CRN == c.CRN, c);
                await scheduleRepo.CreateOrUpdate(x => x.PersonId == savedPerson.Id
                && x.SemesterCode == bannerInfo.SemesterId
                && x.ClassCRN == c.CRN, new Schedule()
                {
                    SemesterCode = bannerInfo.SemesterId,
                    PersonId = savedPerson.Id,
                    ClassCRN = c.CRN
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