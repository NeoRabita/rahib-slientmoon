using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlientMoon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseTranslations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    CourseId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    LanguageCode = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    Subtitle = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseTranslations_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19001",
                column: "CreatedAt",
                value: new DateTime(2026, 8, 16, 15, 14, 41, 207, DateTimeKind.Utc).AddTicks(9709));

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19002",
                column: "CreatedAt",
                value: new DateTime(2026, 8, 16, 15, 14, 41, 207, DateTimeKind.Utc).AddTicks(9716));

            migrationBuilder.CreateIndex(
                name: "IX_CourseTranslations_CourseId_LanguageCode",
                table: "CourseTranslations",
                columns: new[] { "CourseId", "LanguageCode" },
                unique: true,
                filter: "\"CourseId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseTranslations");

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19001",
                column: "CreatedAt",
                value: new DateTime(2026, 8, 5, 15, 43, 19, 566, DateTimeKind.Utc).AddTicks(5762));

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19002",
                column: "CreatedAt",
                value: new DateTime(2026, 8, 5, 15, 43, 19, 566, DateTimeKind.Utc).AddTicks(5770));
        }
    }
}
