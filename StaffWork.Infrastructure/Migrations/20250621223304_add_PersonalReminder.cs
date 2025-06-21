using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffWork.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_PersonalReminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonalReminderId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PersonalReminders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReminderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsReminderCompleted = table.Column<bool>(type: "bit", nullable: false),
                    ReminderCompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JobId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalReminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalReminders_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonalReminderFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonalReminderId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalReminderFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalReminderFiles_PersonalReminders_PersonalReminderId",
                        column: x => x.PersonalReminderId,
                        principalTable: "PersonalReminders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_PersonalReminderId",
                table: "Notifications",
                column: "PersonalReminderId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalReminderFiles_PersonalReminderId",
                table: "PersonalReminderFiles",
                column: "PersonalReminderId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalReminders_CreatedByUserId",
                table: "PersonalReminders",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_PersonalReminders_PersonalReminderId",
                table: "Notifications",
                column: "PersonalReminderId",
                principalTable: "PersonalReminders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_PersonalReminders_PersonalReminderId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "PersonalReminderFiles");

            migrationBuilder.DropTable(
                name: "PersonalReminders");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_PersonalReminderId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "PersonalReminderId",
                table: "Notifications");
        }
    }
}
