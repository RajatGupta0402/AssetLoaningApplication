using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetLoaningApplication.Migrations
{
    /// <inheritdoc />
    public partial class SeventhMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDetails_AssetDetails_assetDetailsassetId",
                table: "TransactionDetails");

            migrationBuilder.DropIndex(
                name: "IX_TransactionDetails_assetDetailsassetId",
                table: "TransactionDetails");

            migrationBuilder.DropColumn(
                name: "assetDetailsassetId",
                table: "TransactionDetails");

            migrationBuilder.AddColumn<Guid>(
                name: "assetId",
                table: "TransactionDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDetails_assetId",
                table: "TransactionDetails",
                column: "assetId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDetails_AssetDetails_assetId",
                table: "TransactionDetails",
                column: "assetId",
                principalTable: "AssetDetails",
                principalColumn: "assetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDetails_AssetDetails_assetId",
                table: "TransactionDetails");

            migrationBuilder.DropIndex(
                name: "IX_TransactionDetails_assetId",
                table: "TransactionDetails");

            migrationBuilder.DropColumn(
                name: "assetId",
                table: "TransactionDetails");

            migrationBuilder.AddColumn<Guid>(
                name: "assetDetailsassetId",
                table: "TransactionDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDetails_assetDetailsassetId",
                table: "TransactionDetails",
                column: "assetDetailsassetId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDetails_AssetDetails_assetDetailsassetId",
                table: "TransactionDetails",
                column: "assetDetailsassetId",
                principalTable: "AssetDetails",
                principalColumn: "assetId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
