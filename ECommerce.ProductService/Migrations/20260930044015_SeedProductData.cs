using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce.ProductService.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Electronics" },
                    { 2, "Books" },
                    { 3, "Clothing" },
                    { 4, "Home & Kitchen" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 1, "15-inch laptop with 16GB RAM and 512GB SSD", "https://placehold.co/600x400?text=Laptop", "Laptop", 75000m },
                    { 2, 1, "Android smartphone with 128GB storage", "https://placehold.co/600x400?text=Smartphone", "Smartphone", 45000m },
                    { 3, 1, "Noise-cancelling wireless headphones", "https://placehold.co/600x400?text=Headphones", "Wireless Headphones", 5000m },
                    { 4, 2, "A book about writing clean and maintainable software", "https://placehold.co/600x400?text=Clean+Code", "Clean Code", 1200m },
                    { 5, 2, "A guide to becoming a better software developer", "https://placehold.co/600x400?text=Pragmatic+Programmer", "The Pragmatic Programmer", 1500m },
                    { 6, 3, "Comfortable round-neck cotton T-shirt", "https://placehold.co/600x400?text=Cotton+T-Shirt", "Cotton T-Shirt", 800m },
                    { 7, 3, "Lightweight shoes suitable for running and training", "https://placehold.co/600x400?text=Running+Shoes", "Running Shoes", 2500m },
                    { 8, 4, "Automatic coffee maker for home use", "https://placehold.co/600x400?text=Coffee+Maker", "Coffee Maker", 3500m }
                });

            migrationBuilder.InsertData(
                table: "Inventories",
                columns: new[] { "Id", "ProductId", "Quantity", "ReservedQuantity" },
                values: new object[,]
                {
                    { 1, 1, 10, 0 },
                    { 2, 2, 25, 0 },
                    { 3, 3, 30, 0 },
                    { 4, 4, 15, 0 },
                    { 5, 5, 20, 0 },
                    { 6, 6, 50, 0 },
                    { 7, 7, 35, 0 },
                    { 8, 8, 12, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 8);

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
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
