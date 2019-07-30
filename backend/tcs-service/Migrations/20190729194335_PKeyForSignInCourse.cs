using Microsoft.EntityFrameworkCore.Migrations;

namespace tcs_service.Migrations
{
    public partial class PKeyForSignInCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SignInCourses_CourseID",
                table: "SignInCourses");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_SignInCourses_CourseID_SignInID",
                table: "SignInCourses",
                columns: new[] { "CourseID", "SignInID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_SignInCourses_CourseID_SignInID",
                table: "SignInCourses");

            migrationBuilder.CreateIndex(
                name: "IX_SignInCourses_CourseID",
                table: "SignInCourses",
                column: "CourseID");
        }
    }
}
