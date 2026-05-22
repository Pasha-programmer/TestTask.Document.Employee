using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestTask.Document.Employee.Database.Migrations
{
    /// <inheritdoc />
    public partial class AuthorAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "author_id",
                table: "document_requests",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "author_id",
                table: "document_requests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
