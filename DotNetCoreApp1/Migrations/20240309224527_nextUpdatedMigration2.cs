using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetCoreApp1.Migrations
{
    /// <inheritdoc />
    public partial class nextUpdatedMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Data_DataId",
                table: "Records");

            migrationBuilder.DropTable(
                name: "Data");

            migrationBuilder.DropIndex(
                name: "IX_Records_DataId",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "DataId",
                table: "Records");

            migrationBuilder.AddColumn<Guid>(
                name: "BookId",
                table: "Records",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LendedFrom",
                table: "Records",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LendedTo",
                table: "Records",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "SendNotification",
                table: "Records",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Book",
                columns: table => new
                {
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Book", x => x.BookId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Records_BookId",
                table: "Records",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Book_BookId",
                table: "Records",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Book_BookId",
                table: "Records");

            migrationBuilder.DropTable(
                name: "Book");

            migrationBuilder.DropIndex(
                name: "IX_Records_BookId",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "LendedFrom",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "LendedTo",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "SendNotification",
                table: "Records");

            migrationBuilder.AddColumn<Guid>(
                name: "DataId",
                table: "Records",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Data",
                columns: table => new
                {
                    DataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data", x => x.DataId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Records_DataId",
                table: "Records",
                column: "DataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Data_DataId",
                table: "Records",
                column: "DataId",
                principalTable: "Data",
                principalColumn: "DataId");
        }
    }
}
