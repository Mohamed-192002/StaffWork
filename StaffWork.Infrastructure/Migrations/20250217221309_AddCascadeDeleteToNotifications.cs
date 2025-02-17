using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffWork.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteToNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Vacations_VacationId",
                table: "Notifications");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Vacations_VacationId",
                table: "Notifications",
                column: "VacationId",
                principalTable: "Vacations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Vacations_VacationId",
                table: "Notifications");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Vacations_VacationId",
                table: "Notifications",
                column: "VacationId",
                principalTable: "Vacations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
