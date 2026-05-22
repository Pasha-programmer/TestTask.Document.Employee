using Microsoft.Extensions.DependencyInjection;
using TestTask.Document.Employee.Infrastructure;

namespace TestTask.Document.Employee.WebApi.Builders;

public static class ServicesBuilder
{
    public static void AddEmployeeDocumentServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region Database

        services.AddEmployeeDocumentDbContext(configuration);

        #endregion

        services.AddEmployeeDocumentBusinessServices(configuration);

        services.AddHealthChecks();
    }
}
