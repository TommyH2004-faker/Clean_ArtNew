using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteIdUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Users_IdUser",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_IdUser",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IdUser",
                table: "Books");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdUser",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_IdUser",
                table: "Books",
                column: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Users_IdUser",
                table: "Books",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
