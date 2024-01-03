using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetLoaningApplication.Migrations
{
    /// <inheritdoc />
    public partial class EighthMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "loanTransactionId",
                table: "TransactionDetails",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "loanTransactionId",
                table: "TransactionDetails");
        }
    }
}
