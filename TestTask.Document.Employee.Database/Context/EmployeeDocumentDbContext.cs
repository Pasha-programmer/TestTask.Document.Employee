using Microsoft.EntityFrameworkCore;
using TestTask.Document.Employee.Database.Entities;

namespace TestTask.Document.Employee.Database.Context;

public class EmployeeDocumentDbContext : DbContext
{
    public EmployeeDocumentDbContext(DbContextOptions<EmployeeDocumentDbContext> optionsBuilder) : base(optionsBuilder) { }

    public DbSet<DocumentRequestEntity> DocumentRequests { get; set; }

    public DbSet<EmployeeEntity> Employes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocumentRequestEntity>(entity =>
        {
            entity.ToTable("document_requests");

            entity.HasKey(e => e.Id).HasName("pk_document_request_id");

            entity.HasIndex(e => new { e.RequestStatus, e.DocumentType })
                .HasDatabaseName("ix_request_status_document_type");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.AuthorId)
                .HasColumnName("author_id");

            entity.Property(e => e.DocumentType)
                .HasColumnName("document_type")
                .HasColumnType("document_type");

            entity.Property(e => e.RequestStatus)
                .HasColumnName("request_status")
                .HasColumnType("document_request_status");
        });

        modelBuilder.Entity<EmployeeEntity>(entity =>
        {
            entity.ToTable("employee");

            entity.HasKey(e => e.Id).HasName("pk_employee_id");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.EmployeeType)
                .HasColumnName("employee_type")
                .HasColumnType("employee_type");

            entity.Property(e => e.PositionType)
                .HasColumnName("position_type")
                .HasColumnType("employee_position_type");
        });
    }
}
