using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TestTask.Document.Employee.Database.Entities.Enums;

#nullable disable

namespace TestTask.Document.Employee.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:document_request_status", "in_process,processed,refusal,sent")
                .Annotation("Npgsql:Enum:document_type", "average_earnings,other,personal_income_tax,work_place_and_seniority")
                .Annotation("Npgsql:Enum:employee_position_type", "accountant,other")
                .Annotation("Npgsql:Enum:employee_type", "employee");

            migrationBuilder.CreateTable(
                name: "document_requests",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    author_id = table.Column<long>(type: "bigint", nullable: false),
                    document_type = table.Column<AccountingDocumentType>(type: "document_type", nullable: false),
                    request_status = table.Column<RequestStatus>(type: "document_request_status", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false),
                    create_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_document_request_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "employee",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    employee_type = table.Column<EmployeeType>(type: "employee_type", nullable: false),
                    position_type = table.Column<PositionType>(type: "employee_position_type", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_request_status_document_type",
                table: "document_requests",
                columns: new[] { "request_status", "document_type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "document_requests");

            migrationBuilder.DropTable(
                name: "employee");
        }
    }
}
