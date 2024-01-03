using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class product_entity_updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTags_Products_PostId",
                table: "ProductTags");

            migrationBuilder.RenameColumn(
                name: "PostId",
                table: "ProductTags",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTags_PostId",
                table: "ProductTags",
                newName: "IX_ProductTags_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTags_Products_ProductId",
                table: "ProductTags",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTags_Products_ProductId",
                table: "ProductTags");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ProductTags",
                newName: "PostId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTags_ProductId",
                table: "ProductTags",
                newName: "IX_ProductTags_PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTags_Products_PostId",
                table: "ProductTags",
                column: "PostId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
