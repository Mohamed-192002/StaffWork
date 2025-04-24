using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffWork.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskUsers_TaskModels_TaskModelId",
                table: "TaskUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskUsers_TaskModels_TaskModelId",
                table: "TaskUsers",
                column: "TaskModelId",
                principalTable: "TaskModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskUsers_TaskModels_TaskModelId",
                table: "TaskUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskUsers_TaskModels_TaskModelId",
                table: "TaskUsers",
                column: "TaskModelId",
                principalTable: "TaskModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
