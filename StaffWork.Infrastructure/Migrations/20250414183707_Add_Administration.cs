using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffWork.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Administration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administration_AspNetUsers_ManagerId",
                table: "Administration");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Administration_AdministrationId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Administration_AdministrationId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkTypes_Administration_AdministrationId",
                table: "WorkTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Administration",
                table: "Administration");

            migrationBuilder.RenameTable(
                name: "Administration",
                newName: "Administrations");

            migrationBuilder.RenameIndex(
                name: "IX_Administration_ManagerId",
                table: "Administrations",
                newName: "IX_Administrations_ManagerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Administrations",
                table: "Administrations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Administrations_AspNetUsers_ManagerId",
                table: "Administrations",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Administrations_AdministrationId",
                table: "Departments",
                column: "AdministrationId",
                principalTable: "Administrations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Administrations_AdministrationId",
                table: "Employees",
                column: "AdministrationId",
                principalTable: "Administrations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTypes_Administrations_AdministrationId",
                table: "WorkTypes",
                column: "AdministrationId",
                principalTable: "Administrations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administrations_AspNetUsers_ManagerId",
                table: "Administrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Administrations_AdministrationId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Administrations_AdministrationId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkTypes_Administrations_AdministrationId",
                table: "WorkTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Administrations",
                table: "Administrations");

            migrationBuilder.RenameTable(
                name: "Administrations",
                newName: "Administration");

            migrationBuilder.RenameIndex(
                name: "IX_Administrations_ManagerId",
                table: "Administration",
                newName: "IX_Administration_ManagerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Administration",
                table: "Administration",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Administration_AspNetUsers_ManagerId",
                table: "Administration",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Administration_AdministrationId",
                table: "Departments",
                column: "AdministrationId",
                principalTable: "Administration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Administration_AdministrationId",
                table: "Employees",
                column: "AdministrationId",
                principalTable: "Administration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTypes_Administration_AdministrationId",
                table: "WorkTypes",
                column: "AdministrationId",
                principalTable: "Administration",
                principalColumn: "Id");
        }
    }
}
