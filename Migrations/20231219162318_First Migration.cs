using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetLoaningApplication.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetDetails",
                columns: table => new
                {
                    assetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    serialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    model = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDetails", x => x.assetId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionDetails",
                columns: table => new
                {
                    transactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    loanedOrReturned = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    assetDetailsassetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    transactionType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionDetails", x => x.transactionId);
                    table.ForeignKey(
                        name: "FK_TransactionDetails_AssetDetails_assetDetailsassetId",
                        column: x => x.assetDetailsassetId,
                        principalTable: "AssetDetails",
                        principalColumn: "assetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDetails",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    firstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.userId);
                    table.ForeignKey(
                        name: "FK_UserDetails_TransactionDetails_userId",
                        column: x => x.userId,
                        principalTable: "TransactionDetails",
                        principalColumn: "transactionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDetails_assetDetailsassetId",
                table: "TransactionDetails",
                column: "assetDetailsassetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDetails");

            migrationBuilder.DropTable(
                name: "TransactionDetails");

            migrationBuilder.DropTable(
                name: "AssetDetails");
        }
    }
}
