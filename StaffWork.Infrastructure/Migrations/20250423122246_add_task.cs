using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffWork.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_task : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VacationId",
                table: "Notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TaskReminderId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaskModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsReceived = table.Column<bool>(type: "bit", nullable: false),
                    DateReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskFile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskModelId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskFile_TaskModel_TaskModelId",
                        column: x => x.TaskModelId,
                        principalTable: "TaskModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskReminder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskModelId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_TaskReminder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskReminder_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskReminder_TaskModel_TaskModelId",
                        column: x => x.TaskModelId,
                        principalTable: "TaskModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskModelId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskUser_TaskModel_TaskModelId",
                        column: x => x.TaskModelId,
                        principalTable: "TaskModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskReminderFile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskReminderId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskReminderFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskReminderFile_TaskReminder_TaskReminderId",
                        column: x => x.TaskReminderId,
                        principalTable: "TaskReminder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TaskReminderId",
                table: "Notifications",
                column: "TaskReminderId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskFile_TaskModelId",
                table: "TaskFile",
                column: "TaskModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskReminder_CreatedByUserId",
                table: "TaskReminder",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskReminder_TaskModelId",
                table: "TaskReminder",
                column: "TaskModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskReminderFile_TaskReminderId",
                table: "TaskReminderFile",
                column: "TaskReminderId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskUser_TaskModelId",
                table: "TaskUser",
                column: "TaskModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskUser_UserId",
                table: "TaskUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_TaskReminder_TaskReminderId",
                table: "Notifications",
                column: "TaskReminderId",
                principalTable: "TaskReminder",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_TaskReminder_TaskReminderId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "TaskFile");

            migrationBuilder.DropTable(
                name: "TaskReminderFile");

            migrationBuilder.DropTable(
                name: "TaskUser");

            migrationBuilder.DropTable(
                name: "TaskReminder");

            migrationBuilder.DropTable(
                name: "TaskModel");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_TaskReminderId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TaskReminderId",
                table: "Notifications");

            migrationBuilder.AlterColumn<int>(
                name: "VacationId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
