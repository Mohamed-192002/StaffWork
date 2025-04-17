using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffWork.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatetask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_TaskReminder_TaskReminderId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskReminder_AspNetUsers_CreatedByUserId",
                table: "TaskReminder");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskReminder_TaskModel_TaskModelId",
                table: "TaskReminder");

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

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "TaskModel");

            migrationBuilder.RenameTable(
                name: "TaskUser",
                newName: "TaskUsers");

            migrationBuilder.RenameTable(
                name: "TaskReminder",
                newName: "TaskReminders");

            migrationBuilder.RenameTable(
                name: "TaskModel",
                newName: "TaskModels");

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

            migrationBuilder.CreateTable(
                name: "TaskFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskFiles_TaskModels_TaskModelId",
                        column: x => x.TaskModelId,
                        principalTable: "TaskModels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskFiles_TaskModelId",
                table: "TaskFiles",
                column: "TaskModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_TaskReminders_TaskReminderId",
                table: "Notifications",
                column: "TaskReminderId",
                principalTable: "TaskReminders",
                principalColumn: "Id");

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

            migrationBuilder.DropTable(
                name: "TaskFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskUsers",
                table: "TaskUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskReminders",
                table: "TaskReminders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskModels",
                table: "TaskModels");

            migrationBuilder.RenameTable(
                name: "TaskUsers",
                newName: "TaskUser");

            migrationBuilder.RenameTable(
                name: "TaskReminders",
                newName: "TaskReminder");

            migrationBuilder.RenameTable(
                name: "TaskModels",
                newName: "TaskModel");

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

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "TaskModel",
                type: "nvarchar(max)",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_TaskReminder_TaskReminderId",
                table: "Notifications",
                column: "TaskReminderId",
                principalTable: "TaskReminder",
                principalColumn: "Id");

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
