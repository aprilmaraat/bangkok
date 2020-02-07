using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bangkok.EF.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "enum.Status",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "tinyint", nullable: false),
                    StatusInitial = table.Column<string>(type: "nvarchar(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enum.Status", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Transaction.Data",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    TransactionDT = table.Column<DateTime>(nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction.Data", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Transaction.Data_enum.Status",
                        column: x => x.Status,
                        principalTable: "enum.Status",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transaction.Data");

            migrationBuilder.DropTable(
                name: "enum.Status");
        }
    }
}
