using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tcs_service.Migrations
{
    public partial class ChangeToDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "OutTime",
                table: "Sessions",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InTime",
                table: "Sessions",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "OutTime",
                table: "Sessions",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "InTime",
                table: "Sessions",
                nullable: false,
                oldClrType: typeof(DateTime));
        }
    }
}
