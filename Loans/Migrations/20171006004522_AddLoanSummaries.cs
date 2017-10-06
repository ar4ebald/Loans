using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Loans.Migrations
{
    public partial class AddLoanSummaries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_LoanSummary_SummaryFromId_SummaryToId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanSummary_AspNetUsers_FromId",
                table: "LoanSummary");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanSummary_AspNetUsers_ToId",
                table: "LoanSummary");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoanSummary",
                table: "LoanSummary");

            migrationBuilder.RenameTable(
                name: "LoanSummary",
                newName: "LoanSummaries");

            migrationBuilder.RenameIndex(
                name: "IX_LoanSummary_ToId",
                table: "LoanSummaries",
                newName: "IX_LoanSummaries_ToId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoanSummaries",
                table: "LoanSummaries",
                columns: new[] { "FromId", "ToId" });

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameTable(
                name: "LoanSummaries",
                newName: "LoanSummary");

            migrationBuilder.RenameIndex(
                name: "IX_LoanSummaries_ToId",
                table: "LoanSummary",
                newName: "IX_LoanSummary_ToId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoanSummary",
                table: "LoanSummary",
                columns: new[] { "FromId", "ToId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_LoanSummary_SummaryFromId_SummaryToId",
                table: "Loans",
                columns: new[] { "SummaryFromId", "SummaryToId" },
                principalTable: "LoanSummary",
                principalColumns: new[] { "FromId", "ToId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanSummary_AspNetUsers_FromId",
                table: "LoanSummary",
                column: "FromId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanSummary_AspNetUsers_ToId",
                table: "LoanSummary",
                column: "ToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
