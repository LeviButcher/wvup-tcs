using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.EF;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Models.ViewModels;
using tcs_service.Services.Interfaces;

namespace tcs_service.Services
{
    public class MockBannerService : IBannerService
    {
        protected TCSContext _db;

        protected MockBannerService()
        {
            _db = new TCSContext();
        }

        public MockBannerService(DbContextOptions options)
        {
            _db = new TCSContext(options);
        }


        public async Task<CourseWithGradeViewModel> GetStudentGrade(int studentId, int crn, int termCode)
        {
            var grades = Enum.GetValues(typeof(Grade));

            var course = await _db.Courses.Include(x => x.Department).FirstAsync(x => x.CRN == crn);

            return new CourseWithGradeViewModel()
            {
                CRN = course.CRN,
                CourseName = course.CourseName,
                DepartmentName = course.Department.Name,
                Grade = (Grade)new Random().Next(grades.Length),
            };
        }

        public DbSet<Person> PersonTable;
        public DbSet<Course> CourseTable;
        StudentInfoViewModel studentInfoViewModel = new StudentInfoViewModel();
        TeacherInfoViewModel teacherInfoViewModel = new TeacherInfoViewModel();

        public async Task<StudentInfoViewModel> GetStudentInfoWithEmail(string studentEmail)
        {
            return await GetStudentInfo(studentInfoViewModel, studentEmail, -1);
        }

        public async Task<StudentInfoViewModel> GetStudentInfoWithID(int studentID)
        {
            return await GetStudentInfo(studentInfoViewModel, " ", studentID);
        }

        private async Task<StudentInfoViewModel> GetStudentInfo(StudentInfoViewModel student, String email, int id)
        {
            PersonTable = _db.People;
            Person newStudent = null;


            if (id == -1)
            {
                newStudent = await PersonTable.Where(x => x.Email == email).FirstOrDefaultAsync();
            }
            else
            {
                newStudent = await PersonTable.Where(x => x.Id == id).FirstOrDefaultAsync();
            }
            if (newStudent == null)
            {
                throw new TCSException("Student information could not be found");
            }

            student.studentEmail = newStudent.Email;
            student.firstName = newStudent.FirstName;
            student.lastName = newStudent.LastName;
            student.studentID = newStudent.Id;
            student.semesterId = 201903;
            student.personType = "Student";

            return await GetCourseInfo(student);

        }

        private async Task<StudentInfoViewModel> GetCourseInfo(StudentInfoViewModel studentInfoViewModel)
        {
            CourseTable = _db.Set<Course>();

            List<Course> schedule = new List<Course>();
            Course first = await CourseTable.Where(x => x.CRN == 1).Include(x => x.Department).FirstAsync();
            Course second = await CourseTable.Where(x => x.CRN == 2).Include(x => x.Department).FirstAsync();
            Course third = await CourseTable.Where(x => x.CRN == 3).Include(x => x.Department).FirstAsync();
            Course fourth = await CourseTable.Where(x => x.CRN == 4).Include(x => x.Department).FirstAsync();
            schedule.Add(first);
            schedule.Add(second);
            schedule.Add(third);
            schedule.Add(fourth);

            studentInfoViewModel.classSchedule = schedule;

            return studentInfoViewModel;
        }

        public async Task<TeacherInfoViewModel> GetTeacherInfoWithEmail(string teacherEmail)
        {
            return await GetTeacherInfo(teacherInfoViewModel, teacherEmail, -1);
        }

        public async Task<TeacherInfoViewModel> GetTeacherInfoWithID(int teacherID)
        {
            return await GetTeacherInfo(teacherInfoViewModel, "", teacherID);
        }

        private async Task<TeacherInfoViewModel> GetTeacherInfo(TeacherInfoViewModel teacher, String email, int id)
        {
            PersonTable = _db.Set<Person>();
            Person newTeacher = null;

            if (id == -1)
            {
                newTeacher = await PersonTable.Where(x => x.Email == email).FirstOrDefaultAsync();
            }
            else
            {
                newTeacher = await PersonTable.Where(x => x.Id == id).FirstOrDefaultAsync();
            }

            if (newTeacher == null)
            {
                throw new TCSException("Teacher could not be found");
            }

            teacher.teacherEmail = newTeacher.Email;
            teacher.firstName = newTeacher.FirstName;
            teacher.lastName = newTeacher.LastName;
            teacher.teacherID = newTeacher.Id;
            teacher.semesterId = 201903;
            teacher.personType = "Teacher";

            return teacher;
        }
    }
}
