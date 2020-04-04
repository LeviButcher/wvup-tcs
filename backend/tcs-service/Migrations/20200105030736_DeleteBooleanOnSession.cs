#pragma warning disable 1591

using Microsoft.EntityFrameworkCore.Migrations;

namespace tcs_service.Migrations {
    public partial class DeleteBooleanOnSession : Migration {
        protected override void Up (MigrationBuilder migrationBuilder) {
            migrationBuilder.AddColumn<bool> (
                name: "Deleted",
                table: "Sessions",
                nullable : false,
                defaultValue : false);
        }

        protected override void Down (MigrationBuilder migrationBuilder) {
            migrationBuilder.DropColumn (
                name: "Deleted",
                table: "Sessions");
        }
    }
}