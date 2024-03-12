using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetCoreApp1.Migrations
{
    /// <inheritdoc />
    public partial class nextUpdatedMigration4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Books_BookId",
                table: "Records");

            migrationBuilder.DropIndex(
                name: "IX_Records_BookId",
                table: "Records");

            migrationBuilder.RenameColumn(
                name: "Uuid",
                table: "Records",
                newName: "RecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecordId",
                table: "Records",
                newName: "Uuid");

            migrationBuilder.CreateIndex(
                name: "IX_Records_BookId",
                table: "Records",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Books_BookId",
                table: "Records",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
