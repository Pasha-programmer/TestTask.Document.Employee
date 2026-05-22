using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TestTask.Document.Employee.Common.Monads;
using TestTask.Document.Employee.Contract.Dtos;
using TestTask.Document.Employee.Contract.Interfaces.Authorization;
using TestTask.Document.Employee.Contract.Interfaces.IdentityConnector;

namespace TestTask.Document.Employee.Infrastructure.Services.Authorization;

/// <summary>
/// Реализация контракта сервиса токена авторизации.
/// </summary>
internal class AuthorizationTokenService(
    ILogger<AuthorizationTokenService> logger,
    IHttpContextAccessor httpContextAccessor,
    IIdentityConnectorService identityConnectorService
    ) : IAuthorizationTokenService
{
    private readonly IIdentityConnectorService _identityConnectorService = identityConnectorService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private readonly ILogger<AuthorizationTokenService> _logger = logger;

    public static readonly string AuthorizationTokenName = "authorization_token";

    /// <summary>
    /// Получить токен авторизации.
    /// </summary>
    /// <param name="authorizationData">Модель данных авторизации.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Результат с моделью токена.</returns>
    public async Task<Result<AuthorizationTokenDto?, IDictionary<string, string[]>>> GetAuthorizationToken(UserCredentialsDto authorizationData, CancellationToken cancellationToken)
    {
        var authorizationToken = await identityConnectorService.GetUserAuthorizationToken(authorizationData.Login, authorizationData.Password, cancellationToken);

        if (authorizationToken == null)
        {
            return Result<AuthorizationTokenDto?, IDictionary<string, string[]>>.Failed("Не удалось авторизоваться");
        }

        return Result<AuthorizationTokenDto?, IDictionary<string, string[]>>.Success(authorizationToken);
    }

    /// <summary>
    /// Валидирировать токен авторизации.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>true - если токен прошел верификацию, иначе false. Null - если токен не обнаружен.</returns>
    public async Task<Result<bool?>> VerificationCurrentUserAccessToken(CancellationToken cancellationToken)
    {
        if (!(_httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(AuthorizationTokenName, out var token) ?? false))
            return Result.Failed("Не удалось найти токен авторизации");

        return await _identityConnectorService.VerificationToken(token, cancellationToken);
    }
}
