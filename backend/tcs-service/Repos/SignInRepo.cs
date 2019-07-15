using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.EF;
using tcs_service.Models;
using tcs_service.Models.ViewModels;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public abstract class SignInRepo : BaseRepo<SignIn>, ISignInRepo
    {

        public SignInRepo(DbContextOptions options) : base(options)
        {

        }

        public async Task<SignIn> Add(SignIn signIn)
        {
            if ( ! await SemesterExists(signIn.SemesterId))
            {
                await AddSemester(signIn.SemesterId);
            }

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
            return await _db.SignIns.SingleOrDefaultAsync(a => a.ID == id);
        }

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
            _db.SignIns.Update(signIn);
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
             var signIn = await _db.SignIns.Where(p => p.PersonId == id)
                .GroupBy(x => x.PersonId)
                .Select(e => e.OrderByDescending(t => t.InTime)).FirstOrDefaultAsync();
               

            return signIn.Last();
        }

        // needs to be async
        public async Task<SignIn> GetMostRecentSignInByEmail(string email)
        {
            var signIn = await _db.SignIns.Where(p => p.Person.Email == email)
               .GroupBy(x => x.PersonId)
               .Select(e => e.OrderByDescending(t => t.InTime)).FirstOrDefaultAsync();


            return signIn.Last();
        }

        public abstract StudentInfoViewModel GetStudentInfoWithEmail(string studentEmail);

        public abstract StudentInfoViewModel GetStudentInfoWithID(int studentID);

        public abstract TeacherInfoViewModel GetTeacherInfoWithEmail(string teacherEmail);
                                             
        public abstract TeacherInfoViewModel GetTeacherInfoWithID(int teacherID);
    }
}
