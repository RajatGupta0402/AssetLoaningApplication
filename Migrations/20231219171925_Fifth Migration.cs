using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetLoaningApplication.Migrations
{
    /// <inheritdoc />
    public partial class FifthMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDetails_TransactionDetails_userId",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "UserDetails");

            migrationBuilder.AddColumn<Guid>(
                name: "studentId",
                table: "TransactionDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "supervisorId",
                table: "TransactionDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDetails_studentId",
                table: "TransactionDetails",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDetails_supervisorId",
                table: "TransactionDetails",
                column: "supervisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDetails_UserDetails_studentId",
                table: "TransactionDetails",
                column: "studentId",
                principalTable: "UserDetails",
                principalColumn: "userId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDetails_UserDetails_supervisorId",
                table: "TransactionDetails",
                column: "supervisorId",
                principalTable: "UserDetails",
                principalColumn: "userId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDetails_UserDetails_studentId",
                table: "TransactionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDetails_UserDetails_supervisorId",
                table: "TransactionDetails");

            migrationBuilder.DropIndex(
                name: "IX_TransactionDetails_studentId",
                table: "TransactionDetails");

            migrationBuilder.DropIndex(
                name: "IX_TransactionDetails_supervisorId",
                table: "TransactionDetails");

            migrationBuilder.DropColumn(
                name: "studentId",
                table: "TransactionDetails");

            migrationBuilder.DropColumn(
                name: "supervisorId",
                table: "TransactionDetails");

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                table: "UserDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDetails_TransactionDetails_userId",
                table: "UserDetails",
                column: "userId",
                principalTable: "TransactionDetails",
                principalColumn: "transactionId");
        }
    }
}
