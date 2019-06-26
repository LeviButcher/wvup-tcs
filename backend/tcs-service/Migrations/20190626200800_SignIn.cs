using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tcs_service.Migrations
{
    public partial class SignIn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassTours",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    DayVisited = table.Column<DateTime>(nullable: false),
                    NumberOfStudents = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTours", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Code = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    PersonType = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Reasons",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reasons", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Semesters",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semesters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CRN = table.Column<int>(nullable: false),
                    DepartmentID = table.Column<int>(nullable: false),
                    CourseName = table.Column<string>(nullable: false),
                    ShortName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CRN);
                    table.ForeignKey(
                        name: "FK_Courses_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SignIns",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PersonId = table.Column<int>(nullable: false),
                    SemesterId = table.Column<int>(nullable: false),
                    InTime = table.Column<DateTime>(nullable: false),
                    OutTime = table.Column<DateTime>(nullable: true),
                    Tutoring = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignIns", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SignIns_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignIns_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SignInCourses",
                columns: table => new
                {
                    CourseID = table.Column<int>(nullable: false),
                    SignInID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignInCourses", x => new { x.SignInID, x.CourseID });
                    table.ForeignKey(
                        name: "FK_SignInCourses_Courses_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "CRN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignInCourses_SignIns_SignInID",
                        column: x => x.SignInID,
                        principalTable: "SignIns",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SignInReasons",
                columns: table => new
                {
                    ReasonID = table.Column<int>(nullable: false),
                    SignInID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignInReasons", x => new { x.SignInID, x.ReasonID });
                    table.ForeignKey(
                        name: "FK_SignInReasons_Reasons_ReasonID",
                        column: x => x.ReasonID,
                        principalTable: "Reasons",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignInReasons_SignIns_SignInID",
                        column: x => x.SignInID,
                        principalTable: "SignIns",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DepartmentID",
                table: "Courses",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_SignInCourses_CourseID",
                table: "SignInCourses",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_SignInReasons_ReasonID",
                table: "SignInReasons",
                column: "ReasonID");

            migrationBuilder.CreateIndex(
                name: "IX_SignIns_PersonId",
                table: "SignIns",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_SignIns_SemesterId",
                table: "SignIns",
                column: "SemesterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassTours");

            migrationBuilder.DropTable(
                name: "SignInCourses");

            migrationBuilder.DropTable(
                name: "SignInReasons");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Reasons");

            migrationBuilder.DropTable(
                name: "SignIns");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Semesters");
        }
    }
}
