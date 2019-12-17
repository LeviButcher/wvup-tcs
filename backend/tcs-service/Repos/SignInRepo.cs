using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Models.ViewModels;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;
using tcs_service.Services.Interfaces;

namespace tcs_service.Repos
{
    public class SignInRepo : BaseRepo<SignIn>, ISignInRepo
    {
        private IBannerService _bannerService;
        public SignInRepo(DbContextOptions options, IBannerService bannerService) : base(options)
        {
            _bannerService = bannerService;
        }

        public async Task<SignIn> Add(SignIn signIn)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(signIn, null, null);
            if (!Validator.TryValidateObject(signIn, context, results, true))
            {
                if (results.Any())
                {
                    throw new Exception(results.ToString());
                }
            }

            await _db.Semesters.FindAsync(signIn.SemesterId);
            if (signIn.SemesterId == default)
            {
                signIn.SemesterId = (await _db.Semesters.LastAsync()).Id;
            }

            signIn.Courses = signIn.Courses.Select(signInCourse =>
            {
                var tracked = _db.Courses.Find(signInCourse.Course.CRN);
                if (tracked != null)
                {
                    signInCourse.CourseID = tracked.CRN;
                }
                return signInCourse;
            }).ToList();

            signIn.Reasons = signIn.Reasons.Select(signInReason =>
            {
                var tracked = _db.Reasons.Find(signInReason.Reason.Id);
                if (tracked != null)
                {
                    signInReason.ReasonID = tracked.Id;
                }
                return signInReason;
            }).ToList();


            await _db.AddAsync(signIn);
            await _db.SaveChangesAsync();
            return signIn;
        }


        public async Task<SignInViewModel> GetSignInViewModel(int id) =>
            await _db.SignIns.Include(x => x.Courses).ThenInclude(x => x.Course)
            .Include(x => x.Reasons).ThenInclude(x => x.Reason)
            .Include(x => x.Person)
            .Include(x => x.Semester)
            .Select(x => new SignInViewModel()
            {
                Email = x.Person.Email,
                Courses = x.Courses.Select(sc => sc.Course).ToList(),
                Reasons = x.Reasons.Select(sr => sr.Reason).ToList(),
                Id = x.ID,
                SemesterId = x.SemesterId,
                Tutoring = x.Tutoring,
                PersonId = x.PersonId,
                InTime = x.InTime,
                OutTime = x.OutTime,
                SemesterName = x.Semester.Name,
            })
            .SingleOrDefaultAsync(a => a.Id == id);



        public async Task<SignIn> Update(SignIn signIn)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(signIn, null, null);
            if (!Validator.TryValidateObject(signIn, context, results, true))
            {
                if (results.Any())
                {
                    throw new Exception(results.ToString());
                }
            }

            var existingSignIn = await _db.SignIns.Include(x => x.Courses).Include(x => x.Reasons).FirstOrDefaultAsync(x => x.ID == signIn.ID);
            _db.Entry(existingSignIn).CurrentValues.SetValues(signIn);

            existingSignIn.Courses = signIn.Courses.Select(signInCourse =>
                new SignInCourse() { SignInID = signIn.ID, CourseID = signInCourse.Course.CRN }
            ).ToList();

            existingSignIn.Reasons = signIn.Reasons.Select(signInReason =>
                new SignInReason() { SignInID = signIn.ID, ReasonID = signInReason.Reason.Id }
            ).ToList();

            _db.SignIns.Update(existingSignIn);
            await _db.SaveChangesAsync();
            return signIn;
        }

        public async Task<Course> AddCourse(Course course)
        {
            await _db.AddAsync(course);
            await _db.SaveChangesAsync();
            return course;
        }

        public async Task<bool> CourseExist(int id)
        {
            return await _db.Courses.AnyAsync(e => e.CRN == id);
        }


        public async Task<Reason> AddReason(Reason reason)
        {
            await _db.AddAsync(reason);
            await _db.SaveChangesAsync();
            return reason;
        }

        public async Task<bool> ReasonExist(int id)
        {
            return await _db.Reasons.AnyAsync(e => e.Id == id);
        }

        public async Task<Department> AddDepartment(Department dept)
        {
            await _db.AddAsync(dept);
            await _db.SaveChangesAsync();
            return dept;
        }

        public async Task<bool> DepartmentExist(int id)
        {
            return await _db.Departments.AnyAsync(e => e.Code == id);
        }



        public async Task<Person> AddPerson(Person person)
        {
            await _db.AddAsync(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> PersonExist(int id)
        {
            return await _db.People.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> PersonExist(string email)
        {
            return await _db.People.AnyAsync(e => e.Email == email);
        }

        private async Task<bool> SemesterExists(int id)
        {
            return await _db.Semesters.AnyAsync(a => a.Id == id);
        }

        private async Task<Semester> AddSemester(int id)
        {
            var found = await _db.Semesters.FindAsync(id);
            if (found != null) return found;
            String name = "";
            if (id % 100 == 01)
            {
                name = "Fall " + id / 100;
            }
            else if (id % 100 == 02)
            {
                name = "Spring " + id / 100;
            }
            else
            {
                name = "Summer " + id / 100;
            }


            var semester = await _db.Semesters.AddAsync(new Semester
            {
                Id = id,
                Name = name
            });
            await _db.SaveChangesAsync();

            return semester.Entity;
        }

        public async Task UpdateNullSignOuts()
        {
            var signIns = _db.SignIns.Where(x => x.OutTime == null && x.InTime != null);
            foreach (SignIn signIn in signIns)
            {
                signIn.OutTime = signIn.InTime.Value.AddHours(2);
                _db.SignIns.Update(signIn);
            }
            await _db.SaveChangesAsync();

            return;
        }

        public async Task<SignIn> GetMostRecentSignInByID(int id)
        {
            var person = await _db.People.Where(x => x.Id == id).FirstOrDefaultAsync();
            var signIn = await GetMostRecentSignIn(person.Id);

            return signIn;
        }

        public async Task<SignIn> GetMostRecentSignInByEmail(string email)
        {
            var person = await _db.People.Where(x => x.Email == email).FirstAsync();
            var signIn = await GetMostRecentSignIn(person.Id);

            return signIn;
        }

        private async Task<SignIn> GetMostRecentSignIn(int personId)
        {
            if (await _db.SignIns.Where(x => x.PersonId == personId).AnyAsync())
            {
                return await _db.SignIns.Include(x => x.Courses).ThenInclude(x => x.Course).Include(x => x.Reasons).ThenInclude(x => x.Reason).Where(p => p.PersonId == personId).LastAsync();
            }
            return null;
        }

        public async Task<StudentInfoViewModel> GetStudentInfoWithEmail(string studentEmail)
        {
            var result = await _bannerService.GetStudentInfoWithEmail(studentEmail);
            var student = new Person()
            {
                Email = result.studentEmail,
                LastName = result.lastName,
                FirstName = result.firstName,
                Id = result.studentID
            };
            await AddOrDoNothingIfExistsPerson(student);
            await AddRangeOrDoNothingIfExistsCourse(result.classSchedule);
            await AddSemester(result.semesterId);

            return result;
        }

        private async Task AddOrDoNothingIfExistsPerson(Person person)
        {
            if (await _db.People.AnyAsync(x => x.Id == person.Id)) return;
            await _db.People.AddAsync(person);
            await _db.SaveChangesAsync();
            return;
        }

        private async Task<int> AddRangeOrDoNothingIfExistsCourse(List<Course> courses)
        {
            foreach (var course in courses)
            {
                var departmentFound = await _db.Departments.FindAsync(course.Department.Code);
                if (departmentFound != null)
                {
                    course.Department = null;
                    course.DepartmentID = departmentFound.Code;
                }

                var found = await _db.Courses.AnyAsync(x => x.CRN == course.CRN);
                if (!found) _db.Courses.Add(course);
            }
            return await _db.SaveChangesAsync();
        }

        public async Task<StudentInfoViewModel> GetStudentInfoWithID(int studentID)
        {
            var result = await _bannerService.GetStudentInfoWithID(studentID);
            var student = new Person()
            {
                Email = result.studentEmail,
                LastName = result.lastName,
                FirstName = result.firstName,
                Id = result.studentID
            };
            await AddOrDoNothingIfExistsPerson(student);
            await AddRangeOrDoNothingIfExistsCourse(result.classSchedule);
            await AddSemester(result.semesterId);
            return result;
        }

        public async Task<TeacherInfoViewModel> GetTeacherInfoWithEmail(string teacherEmail)
        {
            var result = await _bannerService.GetTeacherInfoWithEmail(teacherEmail);
            var teacher = new Person()
            {
                Email = result.teacherEmail,
                LastName = result.lastName,
                FirstName = result.firstName,
                Id = result.teacherID,
                PersonType = PersonType.Teacher
            };
            await AddOrDoNothingIfExistsPerson(teacher);
            await AddSemester(result.semesterId);
            return result;
        }

        public async Task<TeacherInfoViewModel> GetTeacherInfoWithID(int teacherID)
        {
            var result = await _bannerService.GetTeacherInfoWithID(teacherID);
            var teacher = new Person()
            {
                Email = result.teacherEmail,
                LastName = result.lastName,
                FirstName = result.firstName,
                Id = result.teacherID,
                PersonType = PersonType.Teacher
            };
            await AddOrDoNothingIfExistsPerson(teacher);
            await AddSemester(result.semesterId);
            return result;
        }

        protected override IQueryable<SignIn> Include(DbSet<SignIn> set) => set.Include(x => x.Courses).Include(x => x.Reasons);
    }
}
