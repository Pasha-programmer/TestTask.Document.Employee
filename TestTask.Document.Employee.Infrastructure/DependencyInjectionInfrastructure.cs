using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestTask.Document.Employee.Contract.Dtos;
using TestTask.Document.Employee.Contract.Interfaces.Authorization;
using TestTask.Document.Employee.Contract.Interfaces.DocumentProcess;
using TestTask.Document.Employee.Contract.Interfaces.DocumentRequest;
using TestTask.Document.Employee.Contract.Interfaces.IdentityConnector;
using TestTask.Document.Employee.Contract.Validators;
using TestTask.Document.Employee.Database.Context;
using TestTask.Document.Employee.Database.Entities.Enums;
using TestTask.Document.Employee.Infrastructure.Configuration;
using TestTask.Document.Employee.Infrastructure.Services.Authorization;
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

    public static void AddEmployeeDocumentBusinessServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddHttpContextAccessor();

        serviceCollection.AddOptions<AuthSettings>()
            .Bind(configuration.GetSection("AuthSettings"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection.AddScoped<IDocumentProcessCommand, DocumentProcessCommandService>();
        serviceCollection.AddScoped<IDocumentProcessQuery, DocumentProcessQueryService>();
        serviceCollection.AddScoped<IDocumentRequestCommand, DocumentRequestCommandService>();
        serviceCollection.AddScoped<IDocumentRequestQuery, DocumentRequestQueryService>();

        serviceCollection.AddScoped<IAuthorizationTokenService, AuthorizationTokenService>();
        serviceCollection.AddScoped<IIdentityConnectorService, CustomIdentityService>();

        serviceCollection.AddScoped<IValidator<RequestCommandToCreateDto>, RequestCommandToCreateValidator>();
    }

    public static void DbMigrate(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<EmployeeDocumentDbContext>>();

        using var db = factory.CreateDbContext();
        db.Database.Migrate();
    }

    /// <summary>
    /// Промежуточный слой, перенаправляющий токен авторизации из куков в хэдер.
    /// </summary>
    /// <param name="app">Инициализированнаяя модель от <see cref="IApplicationBuilder"/>.</param>
    public static void UseRedirectAuthorizationTokenToHeader(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Cookies.TryGetValue(AuthorizationTokenService.AuthorizationTokenName, out var token))
                context.Request.Headers.Append("Authorization", $"{JwtBearerDefaults.AuthenticationScheme} {token!}");

            await next();
        });
    }
}
