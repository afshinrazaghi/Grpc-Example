using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeterReaderWeb.Migrations
{
    /// <inheritdoc />
    public partial class initdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Readings",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReadingDate",
                value: new DateTime(2023, 8, 7, 8, 22, 56, 66, DateTimeKind.Local).AddTicks(3449));

            migrationBuilder.UpdateData(
                table: "Readings",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReadingDate",
                value: new DateTime(2023, 8, 7, 8, 22, 56, 66, DateTimeKind.Local).AddTicks(3493));

            migrationBuilder.UpdateData(
                table: "Readings",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReadingDate",
                value: new DateTime(2023, 8, 7, 8, 22, 56, 66, DateTimeKind.Local).AddTicks(3500));

            migrationBuilder.UpdateData(
                table: "Readings",
                keyColumn: "Id",
                keyValue: 4,
                column: "ReadingDate",
                value: new DateTime(2023, 8, 7, 8, 22, 56, 66, DateTimeKind.Local).AddTicks(3506));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Readings",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReadingDate",
                value: new DateTime(2019, 9, 24, 23, 38, 57, 515, DateTimeKind.Local).AddTicks(4150));

            migrationBuilder.UpdateData(
                table: "Readings",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReadingDate",
                value: new DateTime(2019, 9, 24, 23, 38, 57, 518, DateTimeKind.Local).AddTicks(8672));

            migrationBuilder.UpdateData(
                table: "Readings",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReadingDate",
                value: new DateTime(2019, 9, 24, 23, 38, 57, 518, DateTimeKind.Local).AddTicks(8719));

            migrationBuilder.UpdateData(
                table: "Readings",
                keyColumn: "Id",
                keyValue: 4,
                column: "ReadingDate",
                value: new DateTime(2019, 9, 24, 23, 38, 57, 518, DateTimeKind.Local).AddTicks(8724));
        }
    }
}
