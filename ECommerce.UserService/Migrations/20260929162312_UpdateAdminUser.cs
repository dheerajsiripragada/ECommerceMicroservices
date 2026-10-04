using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.UserService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEE9Okmcs7JOeWPU2N8YroNmn1t0XwnoHgpTY65QWw35cZJcwIVJUT0OymICu8nXTzQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "YOUR_EXISTING_ADMIN_HASH");
        }
    }
}
