using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestTask.Document.Employee.Contract.Dtos;
using TestTask.Document.Employee.Contract.Dtos.Enums;
using TestTask.Document.Employee.Contract.Interfaces.IdentityConnector;
using TestTask.Document.Employee.Infrastructure.Configuration;

namespace TestTask.Document.Employee.Infrastructure.Services.Authorization;

internal class CustomIdentityService(
    IOptions<AuthSettings> authSettings

    ) : IIdentityConnectorService
{
    private readonly IOptions<AuthSettings> _authSettings = authSettings;

    public Task<AuthorizationTokenDto?> GetUserAuthorizationToken(string userName, string password, CancellationToken cancellationToken = default)
    {
        var role = password == PositionType.Accountant.ToString() ? PositionType.Accountant : PositionType.Other;
        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, role.ToString())
        };

        var jwt = new JwtSecurityToken(
            issuer: _authSettings.Value.Issuer,
            audience: _authSettings.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)), // время действия 2 минуты
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.Value.SecretKey)), SecurityAlgorithms.HmacSha256));

        return Task.FromResult(new AuthorizationTokenDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
        });
    }

    public async Task<bool> VerificationToken(string token, CancellationToken cancellationToken = default)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "example.com",
            ValidateAudience = true,
            ValidAudience = "api/v1",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.Value.SecretKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var validationResult = await tokenHandler.ValidateTokenAsync(token, validationParameters);

        return validationResult.IsValid;
    }
}
