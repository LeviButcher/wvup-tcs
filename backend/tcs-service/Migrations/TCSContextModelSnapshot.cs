﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using tcs_service.EF;

namespace tcs_service.Migrations
{
    [DbContext(typeof(TCSContext))]
    partial class TCSContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("tcs_service.Models.ClassTour", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DayVisited");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("NumberOfStudents");

                    b.HasKey("ID");

                    b.ToTable("ClassTours");
                });

            modelBuilder.Entity("tcs_service.Models.Course", b =>
                {
                    b.Property<int>("CRN");

                    b.Property<string>("CourseName")
                        .IsRequired();

                    b.Property<int>("DepartmentID");

                    b.Property<string>("ShortName")
                        .IsRequired();

                    b.HasKey("CRN");

                    b.HasIndex("DepartmentID");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("tcs_service.Models.Department", b =>
                {
                    b.Property<int>("Code");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Code");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("tcs_service.Models.Person", b =>
                {
                    b.Property<int>("ID");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<int>("PersonType");

                    b.HasKey("ID");

                    b.ToTable("People");
                });

            modelBuilder.Entity("tcs_service.Models.Reason", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Deleted");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("Reasons");
                });

            modelBuilder.Entity("tcs_service.Models.Semester", b =>
                {
                    b.Property<int>("ID");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("Semesters");
                });

            modelBuilder.Entity("tcs_service.Models.SignIn", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("InTime");

                    b.Property<DateTime?>("OutTime");

                    b.Property<int>("PersonId");

                    b.Property<int>("SemesterId");

                    b.Property<bool>("Tutoring");

                    b.HasKey("ID");

                    b.HasIndex("PersonId");

                    b.HasIndex("SemesterId");

                    b.ToTable("SignIns");
                });

            modelBuilder.Entity("tcs_service.Models.SignInCourse", b =>
                {
                    b.Property<int>("SignInID");

                    b.Property<int>("CourseID");

                    b.HasKey("SignInID", "CourseID");

                    b.HasIndex("CourseID");

                    b.ToTable("SignInCourses");
                });

            modelBuilder.Entity("tcs_service.Models.SignInReason", b =>
                {
                    b.Property<int>("SignInID");

                    b.Property<int>("ReasonID");

                    b.HasKey("SignInID", "ReasonID");

                    b.HasIndex("ReasonID");

                    b.ToTable("SignInReasons");
                });

            modelBuilder.Entity("tcs_service.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<byte[]>("PasswordHash");

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.HasKey("ID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("tcs_service.Models.Course", b =>
                {
                    b.HasOne("tcs_service.Models.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("tcs_service.Models.SignIn", b =>
                {
                    b.HasOne("tcs_service.Models.Person", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("tcs_service.Models.Semester", "Semester")
                        .WithMany()
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("tcs_service.Models.SignInCourse", b =>
                {
                    b.HasOne("tcs_service.Models.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("tcs_service.Models.SignIn", "SignIn")
                        .WithMany("Courses")
                        .HasForeignKey("SignInID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("tcs_service.Models.SignInReason", b =>
                {
                    b.HasOne("tcs_service.Models.Reason", "Reason")
                        .WithMany()
                        .HasForeignKey("ReasonID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("tcs_service.Models.SignIn", "SignIn")
                        .WithMany("Reasons")
                        .HasForeignKey("SignInID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
