using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EIIOS.Migrations
{
    /// <inheritdoc />
    public partial class DbPopulate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Allergens",
                columns: new[] { "Id", "Code", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "GLU", null, "Gluten" },
                    { 2, "NUT", null, "Nuts" },
                    { 3, "DUI", null, "Dairy" },
                    { 4, "EGA", null, "Eggs" },
                    { 5, "SOY", null, "Soy" },
                    { 6, "FIS", null, "Fish" },
                    { 7, "SHE", null, "Shellfish" },
                    { 8, "CEL", null, "Celery" },
                    { 9, "MUS", null, "Mustard" },
                    { 10, "SES", null, "Sesame Seeds" },
                    { 11, "LUP", null, "Lupin" },
                    { 12, "SUL", null, "Sulphites" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Appetizers and small plates", 1, true, "Starters" },
                    { 2, "Hearty and filling meals", 2, true, "Main Courses" },
                    { 3, "Sweet treats to finish your meal", 3, true, "Desserts" },
                    { 4, "Drinks and refreshments", 4, true, "Beverages" }
                });

            migrationBuilder.InsertData(
                table: "InventoryItems",
                columns: new[] { "Id", "CreatedAt", "CurrentQuantity", "MinimumQuantity", "Name", "Unit", "UnitCost", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(797), 50m, 5m, "Chicken Breast", "kg", 6.50m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(797) },
                    { 2, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(800), 100m, 10m, "Flour", "kg", 1.20m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(800) },
                    { 3, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(803), 30m, 5m, "Milk", "l", 0.90m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(803) },
                    { 4, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(805), 200m, 20m, "Eggs", "pieces", 0.15m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(806) },
                    { 5, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(808), 40m, 5m, "Lamb Meat", "kg", 9.50m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(808) },
                    { 6, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(810), 30m, 5m, "Beef Mince", "kg", 8.00m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(810) },
                    { 7, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(812), 300m, 50m, "Pita Bread", "pieces", 0.40m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(813) },
                    { 8, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(815), 200m, 30m, "Tortilla Wraps", "pieces", 0.50m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(815) },
                    { 9, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(817), 15m, 2m, "Lettuce", "kg", 1.80m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(818) },
                    { 10, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(820), 20m, 3m, "Tomatoes", "kg", 2.20m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(820) },
                    { 11, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(822), 25m, 3m, "Onions", "kg", 1.50m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(822) },
                    { 12, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(824), 10m, 2m, "Cabbage", "kg", 1.70m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(825) },
                    { 13, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(827), 12m, 2m, "Cucumbers", "kg", 2.00m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(827) },
                    { 14, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(891), 10m, 2m, "Yogurt", "kg", 3.50m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(891) },
                    { 15, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(896), 5m, 1m, "Garlic", "kg", 4.00m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(896) },
                    { 16, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(898), 8m, 2m, "Chili Sauce", "l", 5.00m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(899) },
                    { 17, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(901), 6m, 1m, "Tahini Sauce", "l", 6.00m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(901) },
                    { 18, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(904), 25m, 5m, "French Fries", "kg", 2.50m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(904) },
                    { 19, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(906), 15m, 3m, "Olive Oil", "l", 7.00m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(907) },
                    { 20, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(909), 5m, 1m, "Spice Mix (Kebab Seasoning)", "kg", 12.00m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(909) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "IsEmailVerified", "LastName", "PasswordHash", "Role", "UpdatedAt", "Username" },
                values: new object[] { 1, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(695), "admin@eiios.com", "System", true, true, "Administrator", "1234", "Administrator", new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(696), "admin" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BasePrice", "CreatedAt", "CreatedById", "Description", "ImageUrl", "IsActive", "IsAvailable", "Name", "PreparationTime", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 7.50m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(965), 1, "Grilled chicken pieces served in a tortilla wrap with lettuce, tomatoes, onions, and garlic sauce.", null, true, true, "Chicken Kebab Wrap", 10, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(966) },
                    { 2, 8.50m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(969), 1, "Thin slices of seasoned lamb served in pita bread with fresh vegetables and chili sauce.", null, true, true, "Lamb Doner Kebab", 12, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(969) },
                    { 3, 9.00m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(972), 1, "Spiced beef mince skewers served with fries and tahini sauce.", null, true, true, "Beef Kofta Kebab", 15, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(972) },
                    { 4, 6.50m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(974), 1, "Crispy falafel balls with lettuce, tomatoes, cucumbers, and tahini sauce in a tortilla wrap.", null, true, true, "Falafel Wrap", 8, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(975) },
                    { 5, 7.00m, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(977), 1, "Grilled chicken pieces served over a bed of crispy fries with garlic sauce.", null, true, true, "Chicken & Fries Box", 10, new DateTime(2025, 9, 24, 9, 55, 9, 338, DateTimeKind.Utc).AddTicks(977) }
                });

            migrationBuilder.InsertData(
                table: "ProductAllergens",
                columns: new[] { "AllergenId", "ProductId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 1, 2 },
                    { 3, 2 },
                    { 4, 2 },
                    { 1, 3 },
                    { 10, 3 },
                    { 1, 4 },
                    { 10, 4 },
                    { 3, 5 },
                    { 4, 5 }
                });

            migrationBuilder.InsertData(
                table: "ProductCategories",
                columns: new[] { "CategoryId", "ProductId" },
                values: new object[,]
                {
                    { 2, 1 },
                    { 2, 2 },
                    { 2, 3 },
                    { 2, 4 },
                    { 2, 5 }
                });

            migrationBuilder.InsertData(
                table: "ProductIngredients",
                columns: new[] { "Id", "InventoryItemId", "ProductId", "QuantityNeeded", "Unit" },
                values: new object[,]
                {
                    { 1, 1, 1, 0.20m, "kg" },
                    { 2, 8, 1, 1m, "pieces" },
                    { 3, 9, 1, 0.05m, "kg" },
                    { 4, 10, 1, 0.05m, "kg" },
                    { 5, 11, 1, 0.02m, "kg" },
                    { 6, 15, 1, 0.01m, "kg" },
                    { 7, 5, 2, 0.25m, "kg" },
                    { 8, 7, 2, 1m, "pieces" },
                    { 9, 9, 2, 0.05m, "kg" },
                    { 10, 10, 2, 0.05m, "kg" },
                    { 11, 11, 2, 0.02m, "kg" },
                    { 12, 16, 2, 0.02m, "l" },
                    { 13, 6, 3, 0.25m, "kg" },
                    { 14, 20, 3, 0.01m, "kg" },
                    { 15, 18, 3, 0.15m, "kg" },
                    { 16, 17, 3, 0.02m, "l" },
                    { 17, 8, 4, 1m, "pieces" },
                    { 18, 9, 4, 0.05m, "kg" },
                    { 19, 10, 4, 0.05m, "kg" },
                    { 20, 13, 4, 0.05m, "kg" },
                    { 21, 17, 4, 0.02m, "l" },
                    { 22, 1, 5, 0.20m, "kg" },
                    { 23, 18, 5, 0.20m, "kg" },
                    { 24, 15, 5, 0.01m, "kg" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ProductAllergens",
                keyColumns: new[] { "AllergenId", "ProductId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "ProductAllergens",
                keyColumns: new[] { "AllergenId", "ProductId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "ProductAllergens",
                keyColumns: new[] { "AllergenId", "ProductId" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "ProductAllergens",
                keyColumns: new[] { "AllergenId", "ProductId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "ProductAllergens",
                keyColumns: new[] { "AllergenId", "ProductId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "ProductAllergens",
                keyColumns: new[] { "AllergenId", "ProductId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "ProductAllergens",
                keyColumns: new[] { "AllergenId", "ProductId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "ProductAllergens",
                keyColumns: new[] { "AllergenId", "ProductId" },
                keyValues: new object[] { 10, 3 });

            migrationBuilder.DeleteData(
                table: "ProductAllergens",
                keyColumns: new[] { "AllergenId", "ProductId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "ProductAllergens",
                keyColumns: new[] { "AllergenId", "ProductId" },
                keyValues: new object[] { 10, 4 });

            migrationBuilder.DeleteData(
                table: "ProductAllergens",
                keyColumns: new[] { "AllergenId", "ProductId" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "ProductAllergens",
                keyColumns: new[] { "AllergenId", "ProductId" },
                keyValues: new object[] { 4, 5 });

            migrationBuilder.DeleteData(
                table: "ProductCategories",
                keyColumns: new[] { "CategoryId", "ProductId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "ProductCategories",
                keyColumns: new[] { "CategoryId", "ProductId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "ProductCategories",
                keyColumns: new[] { "CategoryId", "ProductId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "ProductCategories",
                keyColumns: new[] { "CategoryId", "ProductId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "ProductCategories",
                keyColumns: new[] { "CategoryId", "ProductId" },
                keyValues: new object[] { 2, 5 });

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ProductIngredients",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
