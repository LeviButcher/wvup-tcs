using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.EF;
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


        public CourseWithGradeViewModel GetStudentGrade(int studentId, Course course, Department department)
        {
            var grades = Enum.GetValues(typeof(Grade));

            var courseWithGrade = new CourseWithGradeViewModel();
            courseWithGrade.CRN = course.CRN;
            courseWithGrade.CourseName = course.CourseName;
            courseWithGrade.DepartmentName = department.Name;
            courseWithGrade.Grade = (Grade)new Random().Next(grades.Length);

            return courseWithGrade;

            //return new CourseWithGradeViewModel()
            //{
            //    CRN = course.CRN,
            //    CourseName = course.CourseName,
            //    DepartmentName = course.Department.Name,
            //    Grade = (Grade)new Random().Next(grades.Length)
            //};
        }

        public DbSet<Person> PersonTable;
        public DbSet<Course> CourseTable;
        StudentInfoViewModel studentInfoViewModel = new StudentInfoViewModel();
        TeacherInfoViewModel teacherInfoViewModel = new TeacherInfoViewModel();

        public StudentInfoViewModel GetStudentInfoWithEmail(string studentEmail)
        {
            return GetStudentInfo(studentInfoViewModel, studentEmail, -1);
        }

        public StudentInfoViewModel GetStudentInfoWithID(int studentID)
        {
            return GetStudentInfo(studentInfoViewModel, " ", studentID);
        }

        private StudentInfoViewModel GetStudentInfo(StudentInfoViewModel student, String email, int id)
        {
            PersonTable = _db.Set<Person>();
            Person newStudent = null;


            if (id == -1)
            {
                newStudent = PersonTable.Where(x => x.Email == email).First();
            }
            else
            {
                newStudent = PersonTable.Where(x => x.ID == id).First();
            }

            student.studentEmail = newStudent.Email;
            student.firstName = newStudent.FirstName;
            student.lastName = newStudent.LastName;
            student.studentID = newStudent.ID;
            student.semesterId = 201903;

            return GetCourseInfo(student);

        }

        private StudentInfoViewModel GetCourseInfo(StudentInfoViewModel studentInfoViewModel)
        {
            CourseTable = _db.Set<Course>();

            List<Course> schedule = new List<Course>();
            Course first = CourseTable.Where(x => x.CRN == 1).Include(x => x.Department).First();
            Course second = CourseTable.Where(x => x.CRN == 2).Include(x => x.Department).First();
            Course third = CourseTable.Where(x => x.CRN == 3).Include(x => x.Department).First();
            Course fourth = CourseTable.Where(x => x.CRN == 4).Include(x => x.Department).First();
            schedule.Add(first);
            schedule.Add(second);
            schedule.Add(third);
            schedule.Add(fourth);

            studentInfoViewModel.classSchedule = schedule;

            return studentInfoViewModel;
        }

        public TeacherInfoViewModel GetTeacherInfoWithEmail(string teacherEmail)
        {
            return GetTeacherInfo(teacherInfoViewModel, teacherEmail, -1);
        }

        public TeacherInfoViewModel GetTeacherInfoWithID(int teacherID)
        {
            return GetTeacherInfo(teacherInfoViewModel, "", teacherID);
        }

        private TeacherInfoViewModel GetTeacherInfo(TeacherInfoViewModel teacher, String email, int id)
        {
            PersonTable = _db.Set<Person>();
            Person newTeacher = null;

            if (id == -1)
            {
                newTeacher = PersonTable.Where(x => x.Email == email).First();
            }
            else
            {
                newTeacher = PersonTable.Where(x => x.ID == id).First();
            }

            teacher.teacherEmail = newTeacher.Email;
            teacher.firstName = newTeacher.FirstName;
            teacher.lastName = newTeacher.LastName;
            teacher.teacherID = newTeacher.ID;
            teacher.semesterId = 201903;

            return teacher;
        }
    }
}
