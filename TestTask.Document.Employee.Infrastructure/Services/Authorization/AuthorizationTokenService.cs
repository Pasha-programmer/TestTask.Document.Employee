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
        // запрашиваем токен у IdentityConnector
        var identityTokenResult = await _identityConnectorService.GetUserAuthorizationToken(authorizationData.Login, authorizationData.Password, cancellationToken);

        if (identityTokenResult?.AccessToken == null)
            return Result.Failed("Неправильный логин или пароль");

        var token = new AuthorizationTokenDto
        {
            AccessToken = identityTokenResult!.AccessToken,
            RefreshToken = identityTokenResult!.RefreshToken,
        };

        // возвращаем успешный результат
        return token;
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

    /// <summary>
    /// Получить токен авторизации.
    /// </summary>
    /// <param name="token">Токен обновления.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Модель токена.</returns>
    public async Task<Result<AuthorizationTokenDto?, IDictionary<string, string[]>>> RefreshAuthorizationToken(RefreshAccessTokenDto token, CancellationToken cancellationToken)
    {
        // обновляем токен у IdentityConnector
        var identityTokenResult = await _identityConnectorService.RefreshUserAuthorizationToken(token.RefreshToken, cancellationToken);

        if (identityTokenResult == null)
            return Result.Failed("Не удалось обновить токен авторизации");

        // возвращаем успешный результат
        return new AuthorizationTokenDto
        {
            AccessToken = identityTokenResult!.AccessToken,
            RefreshToken = identityTokenResult!.RefreshToken,
        };
    }
}
