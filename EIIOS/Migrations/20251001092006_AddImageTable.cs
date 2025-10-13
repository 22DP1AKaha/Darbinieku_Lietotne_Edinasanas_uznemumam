using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EIIOS.Migrations
{
    /// <inheritdoc />
    public partial class AddImageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Base64Data = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContentType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    AltText = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6064), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6065) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6067), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6068) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6098), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6098) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6100), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6101) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6103), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6103) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6105), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6106) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6108), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6108) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6110), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6111) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6113), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6113) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6115), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6116) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6118), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6118) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6120), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6120) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6122), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6123) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6125), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6125) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6127), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6127) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6129), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6130) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6131), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6132) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6134), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6134) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6136), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6137) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6139), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6139) });

            migrationBuilder.UpdateData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "InventoryItemId", "QuantityNeeded", "Unit" },
                values: new object[] { 17, 0.02m, "l" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ImageId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6175), null, new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6175) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ImageId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6178), null, new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6178) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "ImageId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6181), null, new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6181) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "ImageId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6184), null, new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6184) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "ImageId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6186), null, new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6186) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6003), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6003) });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ImageId",
                table: "Products",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Images_ImageId",
                table: "Products",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Images_ImageId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Products_ImageId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(797), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(797) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(800), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(800) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(803), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(803) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(805), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(806) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(808), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(808) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(810), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(810) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(812), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(813) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(815), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(815) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(817), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(818) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(820), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(820) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(822), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(822) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(824), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(825) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(827), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(827) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(891), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(891) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(896), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(896) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(898), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(899) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(901), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(901) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(904), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(904) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(906), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(907) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(909), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(909) });

            migrationBuilder.UpdateData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "InventoryItemId", "QuantityNeeded", "Unit" },
                values: new object[] { 13, 0.05m, "kg" });

            migrationBuilder.InsertData(
                table: "ProductIngredients",
                columns: new[] { "Id", "InventoryItemId", "ProductId", "QuantityNeeded", "Unit" },
                values: new object[] { 21, 17, 4, 0.02m, "l" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ImageUrl", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(965), null, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(966) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ImageUrl", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(969), null, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(969) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "ImageUrl", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(972), null, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(972) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "ImageUrl", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(974), null, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(975) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "ImageUrl", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(977), null, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(977) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(695), new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(696) });
        }
    }
}
