using System;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;

namespace tcs_service.EF
{
    /// <summary>TCS Context </summary>
    public class TCSContext : DbContext
    {
        /// <summary>ClassTours Table</summary>
        public DbSet<ClassTour> ClassTours { get; set; }

        /// <summary>Users Table</summary>
        public DbSet<User> Users { get; set; }

        /// <summary>People Table</summary>
        public DbSet<Person> People { get; set; }

        /// <summary>Reasons Table</summary>
        public DbSet<Reason> Reasons { get; set; }

        /// <summary>Schedules Table</summary>
        public DbSet<Schedule> Schedules { get; set; }

        /// <summary>Semesters Table</summary>
        public DbSet<Semester> Semesters { get; set; }

        /// <summary>Departments Table</summary>
        public DbSet<Department> Departments { get; set; }

        /// <summary>Sessions Table</summary>
        public DbSet<Session> Sessions { get; set; }

        /// <summary>Classes Table</summary>
        public DbSet<Class> Classes { get; set; }

        /// <summary>SessionReasons Table</summary>
        public DbSet<SessionReason> SessionReasons { get; set; }

        /// <summary>SessionClasses Table</summary>
        public DbSet<SessionClass> SessionClasses { get; set; }

        /// <summary>TCSContext Constructor</summary>
        public TCSContext()
        {

        }

        /// <summary>TCSContext Constructor</summary>
        public TCSContext(DbContextOptions options) : base(options)
        {
            try
            {
                Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        ///<summary>OnConfiguring</summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        ///<summary>Set up Database Table Properties</summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SessionReason>().HasKey(key => new { key.SessionId, key.ReasonId });

            modelBuilder.Entity<SessionClass>().HasKey(key => new { key.SessionId, key.ClassId });

            modelBuilder.Entity<Class>()
                .Property(p => p.CRN)
                .ValueGeneratedNever();

            modelBuilder.Entity<Semester>()
                .Property(p => p.Code)
                .ValueGeneratedNever();

            modelBuilder.Entity<Person>()
                .Property(p => p.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Department>()
                .Property(p => p.Code)
                .ValueGeneratedNever();

            modelBuilder.Entity<Schedule>()
                .HasKey(key => new { key.ClassCRN, key.PersonId, key.SemesterCode });
        }
    }
}