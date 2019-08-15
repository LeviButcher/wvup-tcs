using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            await _db.Semesters.FindAsync(signIn.SemesterId);
            if (signIn.SemesterId == default)
            {
                signIn.SemesterId = (await _db.Semesters.LastAsync()).ID;
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
                var tracked = _db.Reasons.Find(signInReason.Reason.ID);
                if (tracked != null)
                {
                    signInReason.ReasonID = tracked.ID;
                }
                return signInReason;
            }).ToList();


            await _db.AddAsync(signIn);
            await _db.SaveChangesAsync();
            return signIn;
        }

        public async override Task<bool> Exist(int id)
        {
            return await _db.SignIns.AnyAsync(e => e.ID == id);
        }

        public async override Task<SignIn> Find(int id)
        {
            return await _db.SignIns.Include(x => x.Courses).Include(x => x.Reasons).SingleOrDefaultAsync(a => a.ID == id);
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



        public override IEnumerable<SignIn> GetAll()
        {
            return _db.SignIns;
        }

        public async override Task<SignIn> Remove(int id)
        {
            var signIn = await _db.SignIns.SingleAsync(a => a.ID == id);
            _db.SignIns.Remove(signIn);
            await _db.SaveChangesAsync();
            return signIn;
        }

        public async Task<SignIn> Update(SignIn signIn)
        {
            var existingSignIn = await _db.SignIns.Include(x => x.Courses).Include(x => x.Reasons).FirstOrDefaultAsync(x => x.ID == signIn.ID);
            _db.Entry(existingSignIn).CurrentValues.SetValues(signIn);

            existingSignIn.Courses = signIn.Courses.Select(signInCourse =>
                new SignInCourse() { SignInID = signIn.ID, CourseID = signInCourse.Course.CRN }
            ).ToList();

            existingSignIn.Reasons = signIn.Reasons.Select(signInReason =>
                new SignInReason() { SignInID = signIn.ID, ReasonID = signInReason.Reason.ID }
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
            return await _db.Reasons.AnyAsync(e => e.ID == id);
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
            return await _db.People.AnyAsync(e => e.ID == id);
        }

        public async Task<bool> PersonExist(string email)
        {
            return await _db.People.AnyAsync(e => e.Email == email);
        }

        private async Task<bool> SemesterExists(int id)
        {
            return await _db.Semesters.AnyAsync(a => a.ID == id);
        }

        private async Task<Semester> AddSemester(int id)
        {
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
                ID = id,
                Name = name
            });

            return semester.Entity;
        }


        public async Task<SignIn> GetMostRecentSignInByID(int id)
        {
            var person = await _db.People.Where(x => x.ID == id).FirstOrDefaultAsync();
            var signIn = await GetMostRecentSignIn(person.ID);

            return signIn;
        }

        public async Task<SignIn> GetMostRecentSignInByEmail(string email)
        {
            var person = await _db.People.Where(x => x.Email == email).FirstAsync();
            var signIn = await GetMostRecentSignIn(person.ID);

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
                ID = result.studentID
            };
            _db.People.Update(student);
            result.classSchedule.ForEach(course =>
            {
                _db.Courses.Update(course);
            });
            await _db.SaveChangesAsync();
            return result;
        }

        public async Task<StudentInfoViewModel> GetStudentInfoWithID(int studentID)
        {
            var result = await _bannerService.GetStudentInfoWithID(studentID);
            var student = new Person()
            {
                Email = result.studentEmail,
                LastName = result.lastName,
                FirstName = result.firstName,
                ID = result.studentID
            };
            _db.People.Update(student);
            result.classSchedule.ForEach(course =>
            {
                _db.Courses.Update(course);
            });
            await _db.SaveChangesAsync();
            return result;
        }

        public async Task<TeacherInfoViewModel> GetTeacherInfoWithEmail(string teacherEmail)
        {
            var result = await _bannerService.GetTeacherInfoWithEmail(teacherEmail);
            var student = new Person()
            {
                Email = result.teacherEmail,
                LastName = result.lastName,
                FirstName = result.firstName,
                ID = result.teacherID
            };
            _db.People.Update(student);
            await _db.SaveChangesAsync();
            return result;
        }

        public async Task<TeacherInfoViewModel> GetTeacherInfoWithID(int teacherID)
        {
            var result = await _bannerService.GetTeacherInfoWithID(teacherID);
            var student = new Person()
            {
                Email = result.teacherEmail,
                LastName = result.lastName,
                FirstName = result.firstName,
                ID = result.teacherID
            };
            _db.People.Update(student);
            await _db.SaveChangesAsync();
            return result;
        }
    }
}
