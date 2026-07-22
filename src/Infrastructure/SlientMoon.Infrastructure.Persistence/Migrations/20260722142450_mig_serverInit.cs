using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlientMoon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_serverInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19001",
                column: "CreatedAt",
                value: new DateTime(2026, 7, 22, 14, 24, 50, 526, DateTimeKind.Utc).AddTicks(485));

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19002",
                column: "CreatedAt",
                value: new DateTime(2026, 7, 22, 14, 24, 50, 526, DateTimeKind.Utc).AddTicks(492));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19001",
                column: "CreatedAt",
                value: new DateTime(2026, 7, 18, 15, 3, 25, 151, DateTimeKind.Utc).AddTicks(2104));

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19002",
                column: "CreatedAt",
                value: new DateTime(2026, 7, 18, 15, 3, 25, 151, DateTimeKind.Utc).AddTicks(2112));
        }
    }
}
