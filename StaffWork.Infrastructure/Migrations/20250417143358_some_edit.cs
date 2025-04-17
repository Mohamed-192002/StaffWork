using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffWork.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class some_edit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskFiles_TaskModels_TaskModelId",
                table: "TaskFiles");

            migrationBuilder.AlterColumn<int>(
                name: "TaskModelId",
                table: "TaskFiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "TaskFiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "TaskFiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TaskFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFiles_TaskModels_TaskModelId",
                table: "TaskFiles",
                column: "TaskModelId",
                principalTable: "TaskModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskFiles_TaskModels_TaskModelId",
                table: "TaskFiles");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "TaskFiles");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "TaskFiles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TaskFiles");

            migrationBuilder.AlterColumn<int>(
                name: "TaskModelId",
                table: "TaskFiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFiles_TaskModels_TaskModelId",
                table: "TaskFiles",
                column: "TaskModelId",
                principalTable: "TaskModels",
                principalColumn: "Id");
        }
    }
}
