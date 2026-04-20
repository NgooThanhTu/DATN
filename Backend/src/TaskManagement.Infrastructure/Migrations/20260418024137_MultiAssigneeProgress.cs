using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MultiAssigneeProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockReason",
                table: "TaskAssignments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BlockedByUserId",
                table: "TaskAssignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ContributionWeight",
                table: "TaskAssignments",
                type: "float",
                nullable: false,
                defaultValue: 1.0);

            migrationBuilder.AddColumn<double>(
                name: "ProgressPercent",
                table: "TaskAssignments",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProgressUpdatedAt",
                table: "TaskAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignments_BlockedByUserId",
                table: "TaskAssignments",
                column: "BlockedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskAssignments_Users_BlockedByUserId",
                table: "TaskAssignments",
                column: "BlockedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskAssignments_Users_BlockedByUserId",
                table: "TaskAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TaskAssignments_BlockedByUserId",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "BlockReason",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "BlockedByUserId",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "ContributionWeight",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "ProgressPercent",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "ProgressUpdatedAt",
                table: "TaskAssignments");
        }
    }
}
