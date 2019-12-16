using System;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Models.ViewModels;
using tcs_service.Repos.Interfaces;
using tcs_service.Services.Interfaces;

namespace tcs_service.Services
{
    public class MockBannerService : IBannerService
    {
        private readonly IPersonRepo personRepo;
        private readonly ISemesterRepo semesterRepo;
        private readonly ICourseRepo courseRepo;

        public MockBannerService(IPersonRepo personRepo, ISemesterRepo semesterRepo, ICourseRepo courseRepo)
        {
            this.personRepo = personRepo;
            this.semesterRepo = semesterRepo;
            this.courseRepo = courseRepo;
        }

        public async Task<BannerPersonInfo> GetBannerInfo(string identifier)
        {
            var person = await personRepo.Find(x => x.Email == identifier || x.Id.ToString() == identifier);
            if (person is Person)
            {
                var currentSemester = semesterRepo.GetAll().Last();
                var rand = new Random(person.Id);
                var takeCount = Math.Ceiling(rand.NextDouble() * 5) + 1;
                var randomCourses = courseRepo.GetAll().Take((int)takeCount);
                if (person.PersonType == PersonType.Student)
                {
                    return new BannerPersonInfo()
                    {
                        Email = person.Email,
                        FirstName = person.FirstName,
                        LastName = person.LastName,
                        Id = person.Id,
                        Teacher = false,
                        SemesterId = currentSemester.Code,
                        Courses = randomCourses.Select(x => new BannerCourse()
                        {
                            CourseName = x.Name,
                            CRN = x.CRN,
                            ShortName = x.ShortName,
                            Department = new BannerDepartment()
                            {
                                Code = x.Department.Code,
                                Name = x.Department.Name
                            }
                        })
                    };
                }
                else
                {
                    return new BannerPersonInfo()
                    {
                        Email = person.Email,
                        FirstName = person.FirstName,
                        LastName = person.LastName,
                        Id = person.Id,
                        Teacher = true,
                        SemesterId = currentSemester.Code,
                    };
                }
            }
            throw new TCSException($"Person was not found with: {identifier}");
        }


        public async Task<CourseWithGradeViewModel> GetStudentGrade(int studentId, int crn, int termCode)
        {
            var grades = Enum.GetValues(typeof(Grade));

            var course = await courseRepo.Find(x => x.CRN == crn);

            return new CourseWithGradeViewModel()
            {
                CRN = course.CRN,
                CourseName = course.Name,
                DepartmentName = course.Department.Name,
                Grade = (Grade)new Random().Next(grades.Length),
            };
        }
    }
}
