using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestTask.Document.Employee.Contract.Interfaces.DocumentProcess;
using TestTask.Document.Employee.Contract.Interfaces.DocumentRequest;
using TestTask.Document.Employee.Database.Context;
using TestTask.Document.Employee.Database.Entities.Enums;
using TestTask.Document.Employee.Infrastructure.Services.DocumentProcess;
using TestTask.Document.Employee.Infrastructure.Services.DocumentRequest;

namespace TestTask.Document.Employee.Infrastructure;

public static class DependencyInjectionInfrastructure
{
    public static void AddEmployeeDocumentDbContext(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContextFactory<EmployeeDocumentDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString("EmployeeDocumentDB")!,
                o =>
                {
                    o.MapEnum<AccountingDocumentType>("document_type");
                    o.MapEnum<RequestStatus>("document_request_status");
                    o.MapEnum<EmployeeType>("employee_type");
                    o.MapEnum<PositionType>("employee_position_type");
                })
        );
    }

    public static void AddEmployeeDocumentBusinessServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDocumentProcessCommand, DocumentProcessCommandService>();
        serviceCollection.AddScoped<IDocumentProcessQuery, DocumentProcessQueryService>();
        serviceCollection.AddScoped<IDocumentRequestCommand, DocumentRequestCommandService>();
        serviceCollection.AddScoped<IDocumentRequestQuery, DocumentRequestQueryService>();
    }
}
