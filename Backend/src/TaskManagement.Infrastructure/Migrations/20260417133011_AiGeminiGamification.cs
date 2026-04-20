using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AiGeminiGamification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PointTransactions_UserWalletUserId",
                table: "PointTransactions");

            migrationBuilder.AddColumn<string>(
                name: "TransactionType",
                table: "PointTransactions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkTaskId",
                table: "PointTransactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PointTransactions_UserWalletUserId_WorkTaskId_TransactionType",
                table: "PointTransactions",
                columns: new[] { "UserWalletUserId", "WorkTaskId", "TransactionType" });

            migrationBuilder.CreateIndex(
                name: "IX_PointTransactions_WorkTaskId",
                table: "PointTransactions",
                column: "WorkTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_PointTransactions_WorkTasks_WorkTaskId",
                table: "PointTransactions",
                column: "WorkTaskId",
                principalTable: "WorkTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointTransactions_WorkTasks_WorkTaskId",
                table: "PointTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PointTransactions_UserWalletUserId_WorkTaskId_TransactionType",
                table: "PointTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PointTransactions_WorkTaskId",
                table: "PointTransactions");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "PointTransactions");

            migrationBuilder.DropColumn(
                name: "WorkTaskId",
                table: "PointTransactions");

            migrationBuilder.CreateIndex(
                name: "IX_PointTransactions_UserWalletUserId",
                table: "PointTransactions",
                column: "UserWalletUserId");
        }
    }
}
