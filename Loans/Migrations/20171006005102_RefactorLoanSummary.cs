using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Loans.Migrations
{
    public partial class RefactorLoanSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_LoanSummaries_SummaryFromId_SummaryToId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanSummaries_AspNetUsers_FromId",
                table: "LoanSummaries");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanSummaries_AspNetUsers_ToId",
                table: "LoanSummaries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoanSummaries",
                table: "LoanSummaries");

            migrationBuilder.DropIndex(
                name: "IX_LoanSummaries_ToId",
                table: "LoanSummaries");

            migrationBuilder.DropIndex(
                name: "IX_Loans_SummaryFromId_SummaryToId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "FromId",
                table: "LoanSummaries");

            migrationBuilder.DropColumn(
                name: "ToId",
                table: "LoanSummaries");

            migrationBuilder.DropColumn(
                name: "SummaryFromId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "SummaryToId",
                table: "Loans");

            migrationBuilder.AddColumn<int>(
                name: "CreditorId",
                table: "LoanSummaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BorrowerId",
                table: "LoanSummaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SummaryBorrowerId",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SummaryCreditorId",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoanSummaries",
                table: "LoanSummaries",
                columns: new[] { "CreditorId", "BorrowerId" });

            migrationBuilder.CreateIndex(
                name: "IX_LoanSummaries_BorrowerId",
                table: "LoanSummaries",
                column: "BorrowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_SummaryCreditorId_SummaryBorrowerId",
                table: "Loans",
                columns: new[] { "SummaryCreditorId", "SummaryBorrowerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_LoanSummaries_SummaryCreditorId_SummaryBorrowerId",
                table: "Loans",
                columns: new[] { "SummaryCreditorId", "SummaryBorrowerId" },
                principalTable: "LoanSummaries",
                principalColumns: new[] { "CreditorId", "BorrowerId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanSummaries_AspNetUsers_BorrowerId",
                table: "LoanSummaries",
                column: "BorrowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanSummaries_AspNetUsers_CreditorId",
                table: "LoanSummaries",
                column: "CreditorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_LoanSummaries_SummaryCreditorId_SummaryBorrowerId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanSummaries_AspNetUsers_BorrowerId",
                table: "LoanSummaries");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanSummaries_AspNetUsers_CreditorId",
                table: "LoanSummaries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoanSummaries",
                table: "LoanSummaries");

            migrationBuilder.DropIndex(
                name: "IX_LoanSummaries_BorrowerId",
                table: "LoanSummaries");

            migrationBuilder.DropIndex(
                name: "IX_Loans_SummaryCreditorId_SummaryBorrowerId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "CreditorId",
                table: "LoanSummaries");

            migrationBuilder.DropColumn(
                name: "BorrowerId",
                table: "LoanSummaries");

            migrationBuilder.DropColumn(
                name: "SummaryBorrowerId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "SummaryCreditorId",
                table: "Loans");

            migrationBuilder.AddColumn<int>(
                name: "FromId",
                table: "LoanSummaries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ToId",
                table: "LoanSummaries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SummaryFromId",
                table: "Loans",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SummaryToId",
                table: "Loans",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoanSummaries",
                table: "LoanSummaries",
                columns: new[] { "FromId", "ToId" });

            migrationBuilder.CreateIndex(
                name: "IX_LoanSummaries_ToId",
                table: "LoanSummaries",
                column: "ToId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_SummaryFromId_SummaryToId",
                table: "Loans",
                columns: new[] { "SummaryFromId", "SummaryToId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_LoanSummaries_SummaryFromId_SummaryToId",
                table: "Loans",
                columns: new[] { "SummaryFromId", "SummaryToId" },
                principalTable: "LoanSummaries",
                principalColumns: new[] { "FromId", "ToId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanSummaries_AspNetUsers_FromId",
                table: "LoanSummaries",
                column: "FromId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanSummaries_AspNetUsers_ToId",
                table: "LoanSummaries",
                column: "ToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
