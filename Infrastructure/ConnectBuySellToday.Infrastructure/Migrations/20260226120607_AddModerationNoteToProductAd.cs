using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectBuySellToday.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModerationNoteToProductAd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModerationNote",
                table: "ProductAds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 12, 6, 5, 943, DateTimeKind.Utc).AddTicks(7451));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 12, 6, 5, 943, DateTimeKind.Utc).AddTicks(7456));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 12, 6, 5, 943, DateTimeKind.Utc).AddTicks(7460));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 12, 6, 5, 943, DateTimeKind.Utc).AddTicks(7462));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 12, 6, 5, 943, DateTimeKind.Utc).AddTicks(7465));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 12, 6, 5, 943, DateTimeKind.Utc).AddTicks(7468));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 12, 6, 5, 943, DateTimeKind.Utc).AddTicks(7529));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModerationNote",
                table: "ProductAds");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 7, 50, 37, 1, DateTimeKind.Utc).AddTicks(6550));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 7, 50, 37, 1, DateTimeKind.Utc).AddTicks(6554));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 7, 50, 37, 1, DateTimeKind.Utc).AddTicks(6557));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 7, 50, 37, 1, DateTimeKind.Utc).AddTicks(6559));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 7, 50, 37, 1, DateTimeKind.Utc).AddTicks(6562));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 7, 50, 37, 1, DateTimeKind.Utc).AddTicks(6571));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 7, 50, 37, 1, DateTimeKind.Utc).AddTicks(6574));
        }
    }
}
