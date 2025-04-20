using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffWork.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_JopId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "TaskReminders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobId",
                table: "TaskReminders");
        }
    }
}
