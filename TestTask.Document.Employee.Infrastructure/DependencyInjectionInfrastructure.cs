using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestTask.Document.Employee.Database.Context;
using TestTask.Document.Employee.Database.Entities.Enums;

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
}
