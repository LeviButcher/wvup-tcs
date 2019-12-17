using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tcs_service.Migrations
{
    public partial class schedulekey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Semesters");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Semesters",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "SemesterId",
                table: "Schedules",
                newName: "SemesterCode");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "Schedules",
                newName: "CourseCRN");

            migrationBuilder.RenameColumn(
                name: "CourseName",
                table: "Courses",
                newName: "Name");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Schedules"
            );

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Schedules"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules",
                columns: new[] { "CourseCRN", "PersonId", "SemesterCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_PersonId",
                table: "Schedules",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_SemesterCode",
                table: "Schedules",
                column: "SemesterCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Courses_CourseCRN",
                table: "Schedules",
                column: "CourseCRN",
                principalTable: "Courses",
                principalColumn: "CRN",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_People_PersonId",
                table: "Schedules",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Semesters_SemesterCode",
                table: "Schedules",
                column: "SemesterCode",
                principalTable: "Semesters",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Courses_CourseCRN",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_People_PersonId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Semesters_SemesterCode",
                table: "Schedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_PersonId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_SemesterCode",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Semesters",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "SemesterCode",
                table: "Schedules",
                newName: "SemesterId");

            migrationBuilder.RenameColumn(
                name: "CourseCRN",
                table: "Schedules",
                newName: "ClassId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Courses",
                newName: "CourseName");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Semesters",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "PersonId",
                table: "Schedules",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules",
                column: "PersonId");
        }
    }
}
