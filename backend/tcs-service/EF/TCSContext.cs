using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using tcs_service.Models;

namespace tcs_service.EF
{
  public class TCSContext : DbContext
  {
     public DbSet<ClassTour> ClassTours { get; set; }

     public DbSet<User> Users { get; set; }

     public DbSet<Course> Courses { get; set; }
     
     public DbSet<Person> People { get; set; }
     
     public DbSet<Reason> Reasons { get; set; }
     
     public DbSet<SignIn> SignIns { get; set; }
     
     public DbSet<Semester> Semesters { get; set; }
     
     public DbSet<SignInCourse> SignInCourses { get; set; }
     
     public DbSet<SignInReason> SignInReasons { get; set; }
     
     public DbSet<Department> Departments { get; set; }

        public TCSContext()
        {

        }

        public TCSContext(DbContextOptions options) : base(options)
        {
            try
            {
                Database.Migrate();
            }
            catch (Exception ex)
            {
                // logger.Error("Exception running migrations. ", ex);
            }
        }

        internal object Where(Func<object, object> p)
        {
            throw new NotImplementedException();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
            modelBuilder.Entity<SignInCourse>().HasKey(key => new { key.SignInID, key.CourseID });

            modelBuilder.Entity<SignInReason>().HasKey(key => new { key.SignInID, key.ReasonID });

            modelBuilder.Entity<Course>()
            .Property(p => p.CRN)
            .ValueGeneratedNever();

            modelBuilder.Entity<Semester>()
            .Property(p => p.ID)
            .ValueGeneratedNever();

            modelBuilder.Entity<Person>()
             .Property(p => p.ID)
             .ValueGeneratedNever();

            modelBuilder.Entity<Department>()
            .Property(p => p.Code)
            .ValueGeneratedNever();
        }
  }
}
