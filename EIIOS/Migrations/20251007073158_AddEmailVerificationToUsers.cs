using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EIIOS.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerificationToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationToken",
                table: "Users",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerificationTokenExpiry",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6967), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6968) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6970), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6971) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6973), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6974) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6976), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6976) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6978), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6979) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6981), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6981) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6983), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6983) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6985), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6986) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6988), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6988) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6990), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6990) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6992), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6993) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6995), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6995) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6997), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6998) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7000), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7000) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7002), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7003) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7005), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7005) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7007), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7007) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7009), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7009) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7011), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7012) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7014), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7014) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7061), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7062) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7064), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7065) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7067), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7068) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7072), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7072) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7074), new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(7075) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "EmailVerificationToken", "EmailVerificationTokenExpiry", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6911), null, null, new DateTime(2025, 10, 7, 7, 31, 58, 411, DateTimeKind.Utc).AddTicks(6911) });

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailVerificationToken",
                table: "Users",
                column: "EmailVerificationToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_EmailVerificationToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailVerificationToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailVerificationTokenExpiry",
                table: "Users");

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
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6175), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6175) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6178), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6178) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6181), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6181) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6184), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6184) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6186), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6186) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6003), new DateTime(2025, 10, 1, 9, 20, 5, 715, DateTimeKind.Utc).AddTicks(6003) });
        }
    }
}
