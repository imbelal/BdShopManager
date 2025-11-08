using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class expense_entity_updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Users_ApprovedBy",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ApprovedBy",
                table: "Expenses");

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "Expenses",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ApprovedBy",
                table: "Expenses",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ApprovedBy",
                table: "Expenses",
                column: "ApprovedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Users_ApprovedBy",
                table: "Expenses",
                column: "ApprovedBy",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
