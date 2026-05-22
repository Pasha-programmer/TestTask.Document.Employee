using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using TestTask.Document.Employee.Contract.Interfaces.DocumentProcess;
using TestTask.Document.Employee.Contract.Interfaces.DocumentRequest;
using TestTask.Document.Employee.Database.Context;
using TestTask.Document.Employee.Database.Entities.Enums;
using TestTask.Document.Employee.Infrastructure.Services.DocumentProcess;
using TestTask.Document.Employee.Infrastructure.Services.DocumentRequest;
using Microsoft.AspNetCore.Builder;
using TestTask.Document.Employee.Contract.Interfaces.Authorization;
using TestTask.Document.Employee.Infrastructure.Services.Authorization;
using TestTask.Document.Employee.Contract.Interfaces.IdentityConnector;
using TestTask.Document.Employee.Infrastructure.Services.Keycloak;
using TestTask.Document.Employee.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

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

        serviceCollection.AddHttpClient(KeycloakService.NO_AUTH_HTTP_CLIENT_NAME, (serviceProvider, httpClient) =>
        {
            var keycloakSettings = serviceProvider.GetRequiredService<IOptions<KeycloakConfiguration>>();

            httpClient.BaseAddress = keycloakSettings.Value.ServerUrl;
        });

        serviceCollection.AddHttpClient(KeycloakService.ADMIN_HTTP_CLIENT_NAME, (serviceProvider, httpClient) =>
        {
            var keycloakSettings = serviceProvider.GetRequiredService<IOptions<KeycloakConfiguration>>();

            httpClient.BaseAddress = new Uri($"{keycloakSettings.Value.ServerUrl}/admin/");
        }).AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

        serviceCollection.AddOptions<KeycloakConfiguration>()
            .Bind(configuration.GetSection("KeycloakSettings"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection.AddOptions<AdminCredentialsConfiguration>()
            .Bind(configuration.GetSection("KeycloakSettings:AdminCredentials"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection.AddScoped<IDocumentProcessCommand, DocumentProcessCommandService>();
        serviceCollection.AddScoped<IDocumentProcessQuery, DocumentProcessQueryService>();
        serviceCollection.AddScoped<IDocumentRequestCommand, DocumentRequestCommandService>();
        serviceCollection.AddScoped<IDocumentRequestQuery, DocumentRequestQueryService>();

        serviceCollection.AddScoped<IAuthorizationTokenService, AuthorizationTokenService>();
        serviceCollection.AddScoped<IIdentityConnectorService, KeycloakService>();
    }

    public static void DbMigrate(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<EmployeeDocumentDbContext>>();

        using var db = factory.CreateDbContext();
        db.Database.Migrate();
    }

    public static void AddJwtAuthentication(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var authority = configuration.GetValue<string>("KeycloakSettings:Authority");
        var audience = configuration.GetValue<string>("KeycloakSettings:Audience");

        if (string.IsNullOrWhiteSpace(authority) || string.IsNullOrWhiteSpace(audience))
            throw new ArgumentException("Конфигурация аутентификации не валидна, требуются переменные в окружении типа string с именами KeycloakSettings:Authority и KeycloakSettings:Audience.");

        serviceCollection
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authority,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = GetIssuerSigningKeys(configuration),
                };
                options.RequireHttpsMetadata = false;// configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") != "Development";
            });
    }

    public static void AddRequireAuthorization(IServiceCollection serviceCollection)
    {
        serviceCollection.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
              .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
              .RequireAuthenticatedUser()
              .Build();
        });
    }

    private static IReadOnlyCollection<SecurityKey> GetIssuerSigningKeys(IConfiguration configuration)
    {
        var keycloakServer = configuration.GetValue<string>("KeycloakSettings:ServerUrl");
        var keycloakRealm = configuration.GetValue<string>("KeycloakSettings:RealmId");
        var keycloakJWKSUrl = $"{keycloakServer}/realms/{keycloakRealm}/protocol/openid-connect/certs";

        var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            keycloakJWKSUrl,
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever()
            {
                RequireHttps = false
            }
        );

        var config = configurationManager.GetConfigurationAsync(CancellationToken.None).GetAwaiter().GetResult();
        var additionalData = config.AdditionalData.Values;

        var keys = new List<SecurityKey>(2);

        foreach (JsonElement value in additionalData)
        {
            foreach (var key in value.EnumerateArray())
            {
                var jwkJson = key.ToString();
                var jwk = JsonWebKey.Create(jwkJson);

                if (jwk.Kty == "RSA")
                    keys.Add(jwk);
                else
                    throw new Exception($"Unsupported key type: {jwk.Kty}"); //Нужно ли?
            }
        }
        return keys;
    }
}
