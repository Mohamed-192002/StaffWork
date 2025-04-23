using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffWork.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_task2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_TaskReminder_TaskReminderId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskFile_TaskModel_TaskModelId",
                table: "TaskFile");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskReminder_AspNetUsers_CreatedByUserId",
                table: "TaskReminder");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskReminder_TaskModel_TaskModelId",
                table: "TaskReminder");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskReminderFile_TaskReminder_TaskReminderId",
                table: "TaskReminderFile");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskUser_AspNetUsers_UserId",
                table: "TaskUser");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskUser_TaskModel_TaskModelId",
                table: "TaskUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskUser",
                table: "TaskUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskReminder",
                table: "TaskReminder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskModel",
                table: "TaskModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskFile",
                table: "TaskFile");

            migrationBuilder.RenameTable(
                name: "TaskUser",
                newName: "TaskUsers");

            migrationBuilder.RenameTable(
                name: "TaskReminder",
                newName: "TaskReminders");

            migrationBuilder.RenameTable(
                name: "TaskModel",
                newName: "TaskModels");

            migrationBuilder.RenameTable(
                name: "TaskFile",
                newName: "TaskFiles");

            migrationBuilder.RenameIndex(
                name: "IX_TaskUser_UserId",
                table: "TaskUsers",
                newName: "IX_TaskUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskUser_TaskModelId",
                table: "TaskUsers",
                newName: "IX_TaskUsers_TaskModelId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskReminder_TaskModelId",
                table: "TaskReminders",
                newName: "IX_TaskReminders_TaskModelId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskReminder_CreatedByUserId",
                table: "TaskReminders",
                newName: "IX_TaskReminders_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskFile_TaskModelId",
                table: "TaskFiles",
                newName: "IX_TaskFiles_TaskModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskUsers",
                table: "TaskUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskReminders",
                table: "TaskReminders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskModels",
                table: "TaskModels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskFiles",
                table: "TaskFiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_TaskReminders_TaskReminderId",
                table: "Notifications",
                column: "TaskReminderId",
                principalTable: "TaskReminders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFiles_TaskModels_TaskModelId",
                table: "TaskFiles",
                column: "TaskModelId",
                principalTable: "TaskModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskReminderFile_TaskReminders_TaskReminderId",
                table: "TaskReminderFile",
                column: "TaskReminderId",
                principalTable: "TaskReminders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskReminders_AspNetUsers_CreatedByUserId",
                table: "TaskReminders",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskReminders_TaskModels_TaskModelId",
                table: "TaskReminders",
                column: "TaskModelId",
                principalTable: "TaskModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskUsers_AspNetUsers_UserId",
                table: "TaskUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskUsers_TaskModels_TaskModelId",
                table: "TaskUsers",
                column: "TaskModelId",
                principalTable: "TaskModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_TaskReminders_TaskReminderId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskFiles_TaskModels_TaskModelId",
                table: "TaskFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskReminderFile_TaskReminders_TaskReminderId",
                table: "TaskReminderFile");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskReminders_AspNetUsers_CreatedByUserId",
                table: "TaskReminders");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskReminders_TaskModels_TaskModelId",
                table: "TaskReminders");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskUsers_AspNetUsers_UserId",
                table: "TaskUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskUsers_TaskModels_TaskModelId",
                table: "TaskUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskUsers",
                table: "TaskUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskReminders",
                table: "TaskReminders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskModels",
                table: "TaskModels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskFiles",
                table: "TaskFiles");

            migrationBuilder.RenameTable(
                name: "TaskUsers",
                newName: "TaskUser");

            migrationBuilder.RenameTable(
                name: "TaskReminders",
                newName: "TaskReminder");

            migrationBuilder.RenameTable(
                name: "TaskModels",
                newName: "TaskModel");

            migrationBuilder.RenameTable(
                name: "TaskFiles",
                newName: "TaskFile");

            migrationBuilder.RenameIndex(
                name: "IX_TaskUsers_UserId",
                table: "TaskUser",
                newName: "IX_TaskUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskUsers_TaskModelId",
                table: "TaskUser",
                newName: "IX_TaskUser_TaskModelId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskReminders_TaskModelId",
                table: "TaskReminder",
                newName: "IX_TaskReminder_TaskModelId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskReminders_CreatedByUserId",
                table: "TaskReminder",
                newName: "IX_TaskReminder_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskFiles_TaskModelId",
                table: "TaskFile",
                newName: "IX_TaskFile_TaskModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskUser",
                table: "TaskUser",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskReminder",
                table: "TaskReminder",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskModel",
                table: "TaskModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskFile",
                table: "TaskFile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_TaskReminder_TaskReminderId",
                table: "Notifications",
                column: "TaskReminderId",
                principalTable: "TaskReminder",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFile_TaskModel_TaskModelId",
                table: "TaskFile",
                column: "TaskModelId",
                principalTable: "TaskModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskReminder_AspNetUsers_CreatedByUserId",
                table: "TaskReminder",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskReminder_TaskModel_TaskModelId",
                table: "TaskReminder",
                column: "TaskModelId",
                principalTable: "TaskModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskReminderFile_TaskReminder_TaskReminderId",
                table: "TaskReminderFile",
                column: "TaskReminderId",
                principalTable: "TaskReminder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskUser_AspNetUsers_UserId",
                table: "TaskUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskUser_TaskModel_TaskModelId",
                table: "TaskUser",
                column: "TaskModelId",
                principalTable: "TaskModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
