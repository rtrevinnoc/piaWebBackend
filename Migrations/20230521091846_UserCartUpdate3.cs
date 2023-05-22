using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tienda.Migrations
{
    /// <inheritdoc />
    public partial class UserCartUpdate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoughtProducts_Products_ProductId",
                table: "BoughtProducts");

            migrationBuilder.DropIndex(
                name: "IX_BoughtProducts_ProductId",
                table: "BoughtProducts");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "BoughtProducts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "BoughtProducts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "BoughtProducts");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "BoughtProducts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_BoughtProducts_ProductId",
                table: "BoughtProducts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoughtProducts_Products_ProductId",
                table: "BoughtProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
