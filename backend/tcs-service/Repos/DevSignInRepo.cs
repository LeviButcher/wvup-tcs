using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using tcs_service.EF;
using tcs_service.Models;
using tcs_service.Models.ViewModels;

namespace tcs_service.Repos
{
    public class DevSignInRepo : SignInRepo
    {
        public DevSignInRepo(DbContextOptions options) : base(options) { }

        public DbSet<Person> PersonTable;
        public DbSet<Course> CourseTable;
        StudentInfoViewModel studentInfoViewModel = new StudentInfoViewModel();

        public override StudentInfoViewModel GetStudentInfoWithEmail(string studentEmail)
        {
            return GetStudentInfo(studentInfoViewModel, studentEmail, -1);
        }

        public override StudentInfoViewModel GetStudentInfoWithID(int studentID)
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
            Course first = CourseTable.Where(x => x.CRN == 1).First();
            Course second = CourseTable.Where(x => x.CRN == 2).First();
            Course third = CourseTable.Where(x => x.CRN == 3).First();
            Course fourth = CourseTable.Where(x => x.CRN == 44).First();
            schedule.Add(first);
            schedule.Add(second);
            schedule.Add(third);
            schedule.Add(fourth);

            studentInfoViewModel.classSchedule = schedule;

            return studentInfoViewModel;
        }
    }
}

