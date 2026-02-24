using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConnectBuySellToday.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "IconUrl", "Name", "ParentCategoryId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 2, 24, 20, 31, 15, 57, DateTimeKind.Utc).AddTicks(4495), null, "Electronics", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 2, 24, 20, 31, 15, 57, DateTimeKind.Utc).AddTicks(4498), null, "Vehicles", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 2, 24, 20, 31, 15, 57, DateTimeKind.Utc).AddTicks(4500), null, "Furniture", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 2, 24, 20, 31, 15, 57, DateTimeKind.Utc).AddTicks(4502), null, "Clothing", null, null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 2, 24, 20, 31, 15, 57, DateTimeKind.Utc).AddTicks(4513), null, "Books", null, null },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2026, 2, 24, 20, 31, 15, 57, DateTimeKind.Utc).AddTicks(4516), null, "Sports", null, null },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2026, 2, 24, 20, 31, 15, 57, DateTimeKind.Utc).AddTicks(4518), null, "Other", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));
        }
    }
}
