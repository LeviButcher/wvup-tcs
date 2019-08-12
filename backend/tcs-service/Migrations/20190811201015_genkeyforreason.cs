using Microsoft.EntityFrameworkCore.Migrations;

namespace tcs_service.Migrations
{
    public partial class genkeyforreason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SignInReasons_ReasonID",
                table: "SignInReasons");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_SignInReasons_ReasonID_SignInID",
                table: "SignInReasons",
                columns: new[] { "ReasonID", "SignInID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_SignInReasons_ReasonID_SignInID",
                table: "SignInReasons");

            migrationBuilder.CreateIndex(
                name: "IX_SignInReasons_ReasonID",
                table: "SignInReasons",
                column: "ReasonID");
        }
    }
}
