using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Loans.Migrations
{
    public partial class AddLoanSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_BorrowerId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_DebtorId",
                table: "Loans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Loans",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_DebtorId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "BorrowerId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "DebtorId",
                table: "Loans");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "SummaryFromId",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SummaryId",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SummaryToId",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Loans",
                table: "Loans",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LoanSummary",
                columns: table => new
                {
                    FromId = table.Column<int>(type: "int", nullable: false),
                    ToId = table.Column<int>(type: "int", nullable: false),
                    TotalDebt = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanSummary", x => new { x.FromId, x.ToId });
                    table.ForeignKey(
                        name: "FK_LoanSummary_AspNetUsers_FromId",
                        column: x => x.FromId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanSummary_AspNetUsers_ToId",
                        column: x => x.ToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Loans_SummaryFromId_SummaryToId",
                table: "Loans",
                columns: new[] { "SummaryFromId", "SummaryToId" });

            migrationBuilder.CreateIndex(
                name: "IX_LoanSummary_ToId",
                table: "LoanSummary",
                column: "ToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_LoanSummary_SummaryFromId_SummaryToId",
                table: "Loans",
                columns: new[] { "SummaryFromId", "SummaryToId" },
                principalTable: "LoanSummary",
                principalColumns: new[] { "FromId", "ToId" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_LoanSummary_SummaryFromId_SummaryToId",
                table: "Loans");

            migrationBuilder.DropTable(
                name: "LoanSummary");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Loans",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_SummaryFromId_SummaryToId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "SummaryFromId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "SummaryId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "SummaryToId",
                table: "Loans");

            migrationBuilder.AddColumn<int>(
                name: "BorrowerId",
                table: "Loans",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DebtorId",
                table: "Loans",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Loans",
                table: "Loans",
                columns: new[] { "BorrowerId", "DebtorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Loans_DebtorId",
                table: "Loans",
                column: "DebtorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_BorrowerId",
                table: "Loans",
                column: "BorrowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_DebtorId",
                table: "Loans",
                column: "DebtorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
