using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace belajar_netapi.Migrations
{
    /// <inheritdoc />
    public partial class RolesSeeded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2f7a31be-e35b-4569-bee1-c34bbdca3eb2", "2", "User", "User" },
                    { "4edbabd7-d9ef-4beb-a177-f35629381e65", "3", "Moderator", "Moderator" },
                    { "669cec32-afa0-4430-82d4-71d367eefc87", "1", "Admin", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f7a31be-e35b-4569-bee1-c34bbdca3eb2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4edbabd7-d9ef-4beb-a177-f35629381e65");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "669cec32-afa0-4430-82d4-71d367eefc87");
        }
    }
}
