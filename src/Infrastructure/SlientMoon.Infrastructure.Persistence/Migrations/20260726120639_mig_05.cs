using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlientMoon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Narrators",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Gender = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Narrators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Title = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Subtitle = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Type = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ImageUrl = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DurationSec = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IsFeatured = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ViewCount = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CategoryId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseNarrators",
                columns: table => new
                {
                    CourseId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    NarratorId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseNarrators", x => new { x.CourseId, x.NarratorId });
                    table.ForeignKey(
                        name: "FK_CourseNarrators_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseNarrators_Narrators_NarratorId",
                        column: x => x.NarratorId,
                        principalTable: "Narrators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyThoughts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CourseId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyThoughts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyThoughts_Courses_CourseId",
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
                value: new DateTime(2026, 7, 26, 12, 6, 38, 888, DateTimeKind.Utc).AddTicks(1429));

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19002",
                column: "CreatedAt",
                value: new DateTime(2026, 7, 26, 12, 6, 38, 888, DateTimeKind.Utc).AddTicks(1441));

            migrationBuilder.CreateIndex(
                name: "IX_CourseNarrators_NarratorId",
                table: "CourseNarrators",
                column: "NarratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CategoryId",
                table: "Courses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyThoughts_CourseId",
                table: "DailyThoughts",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseNarrators");

            migrationBuilder.DropTable(
                name: "DailyThoughts");

            migrationBuilder.DropTable(
                name: "Narrators");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Categories");

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
    }
}
