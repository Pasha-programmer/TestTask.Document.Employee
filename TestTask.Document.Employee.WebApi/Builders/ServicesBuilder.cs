using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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

        var issuer = configuration.GetValue<string>("AuthSettings:Issuer");
        var audience = configuration.GetValue<string>("AuthSettings:Audience");
        var secretKey = configuration.GetValue<string>("AuthSettings:SecretKey");

        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // указывает, будет ли валидироваться издатель при валидации токена
                    ValidateIssuer = true,
                    // строка, представляющая издателя
                    ValidIssuer = issuer!,
                    // будет ли валидироваться потребитель токена
                    ValidateAudience = true,
                    // установка потребителя токена
                    ValidAudience = audience!,
                    // будет ли валидироваться время существования
                    ValidateLifetime = true,
                    // установка ключа безопасности
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                    // валидация ключа безопасности
                    ValidateIssuerSigningKey = true,
                };
            });

        services.AddHealthChecks();
    }
}
