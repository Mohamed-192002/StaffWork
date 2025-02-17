using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffWork.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_CustomNotifi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomNotifiBeforeDays",
                table: "Vacations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CustomNotifiDate",
                table: "Vacations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CustomNotifiDuration",
                table: "Vacations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoNotifi",
                table: "Vacations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomNotifiBeforeDays",
                table: "Vacations");

            migrationBuilder.DropColumn(
                name: "CustomNotifiDate",
                table: "Vacations");

            migrationBuilder.DropColumn(
                name: "CustomNotifiDuration",
                table: "Vacations");

            migrationBuilder.DropColumn(
                name: "IsAutoNotifi",
                table: "Vacations");
        }
    }
}
