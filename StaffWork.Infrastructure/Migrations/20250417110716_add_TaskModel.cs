using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffWork.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_TaskModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsReceived = table.Column<bool>(type: "bit", nullable: false),
                    DateReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskReminder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskModelId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReminderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsReminderCompleted = table.Column<bool>(type: "bit", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TaskReminderId",
                table: "Notifications",
                column: "TaskReminderId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskReminder_CreatedByUserId",
                table: "TaskReminder",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskReminder_TaskModelId",
                table: "TaskReminder",
                column: "TaskModelId");

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
                name: "TaskReminder");

            migrationBuilder.DropTable(
                name: "TaskUser");

            migrationBuilder.DropTable(
                name: "TaskModel");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_TaskReminderId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TaskReminderId",
                table: "Notifications");
        }
    }
}
