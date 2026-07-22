using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlientMoon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Time",
                table: "Reminders",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(5)",
                oldMaxLength: 5);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Time",
                table: "Reminders",
                type: "NVARCHAR2(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19001",
                column: "CreatedAt",
                value: new DateTime(2026, 7, 16, 10, 0, 23, 108, DateTimeKind.Utc).AddTicks(9324));

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19002",
                column: "CreatedAt",
                value: new DateTime(2026, 7, 16, 10, 0, 23, 108, DateTimeKind.Utc).AddTicks(9332));
        }
    }
}
