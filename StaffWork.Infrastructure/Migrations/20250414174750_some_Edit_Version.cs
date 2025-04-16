using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffWork.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class some_Edit_Version : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdministrationId",
                table: "WorkTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "WorkTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdministrationId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdministrationId",
                table: "Departments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentAdminId",
                table: "Departments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdministrationId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentAdminId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Administration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManagerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Administration_AspNetUsers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DepartmentAdmin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    AdminId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentAdmin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentAdmin_AspNetUsers_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkTypes_AdministrationId",
                table: "WorkTypes",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTypes_DepartmentId",
                table: "WorkTypes",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_AdministrationId",
                table: "Employees",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_AdministrationId",
                table: "Departments",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentAdminId",
                table: "Departments",
                column: "DepartmentAdminId",
                unique: true,
                filter: "[DepartmentAdminId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DepartmentAdminId",
                table: "AspNetUsers",
                column: "DepartmentAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Administration_ManagerId",
                table: "Administration",
                column: "ManagerId",
                unique: true,
                filter: "[ManagerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentAdmin_AdminId",
                table: "DepartmentAdmin",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_DepartmentAdmin_DepartmentAdminId",
                table: "AspNetUsers",
                column: "DepartmentAdminId",
                principalTable: "DepartmentAdmin",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Administration_AdministrationId",
                table: "Departments",
                column: "AdministrationId",
                principalTable: "Administration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_DepartmentAdmin_DepartmentAdminId",
                table: "Departments",
                column: "DepartmentAdminId",
                principalTable: "DepartmentAdmin",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Administration_AdministrationId",
                table: "Employees",
                column: "AdministrationId",
                principalTable: "Administration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTypes_Administration_AdministrationId",
                table: "WorkTypes",
                column: "AdministrationId",
                principalTable: "Administration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTypes_Departments_DepartmentId",
                table: "WorkTypes",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_DepartmentAdmin_DepartmentAdminId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Administration_AdministrationId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_DepartmentAdmin_DepartmentAdminId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Administration_AdministrationId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkTypes_Administration_AdministrationId",
                table: "WorkTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkTypes_Departments_DepartmentId",
                table: "WorkTypes");

            migrationBuilder.DropTable(
                name: "Administration");

            migrationBuilder.DropTable(
                name: "DepartmentAdmin");

            migrationBuilder.DropIndex(
                name: "IX_WorkTypes_AdministrationId",
                table: "WorkTypes");

            migrationBuilder.DropIndex(
                name: "IX_WorkTypes_DepartmentId",
                table: "WorkTypes");

            migrationBuilder.DropIndex(
                name: "IX_Employees_AdministrationId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Departments_AdministrationId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_DepartmentAdminId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DepartmentAdminId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AdministrationId",
                table: "WorkTypes");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "WorkTypes");

            migrationBuilder.DropColumn(
                name: "AdministrationId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "AdministrationId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DepartmentAdminId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "AdministrationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DepartmentAdminId",
                table: "AspNetUsers");
        }
    }
}
