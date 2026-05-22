using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RadioJune.IdentityConnector.Contract.Dtos;
using System.Net.Http.Json;
using TestTask.Document.Employee.Contract.Dtos;
using TestTask.Document.Employee.Contract.Interfaces.IdentityConnector;
using TestTask.Document.Employee.Infrastructure.Configuration;

namespace TestTask.Document.Employee.Infrastructure.Services.Keycloak;

/// <summary>
/// Сервис доступа к апи Keycloak.
/// </summary>
/// <param name="httpClientFactory"></param>
/// <param name="configuration"></param>
internal class KeycloakService(
    IHttpClientFactory httpClientFactory,
    ILogger<KeycloakService> logger,
    IOptions<KeycloakConfiguration> keycloakSettings
    ) : IIdentityConnectorService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<KeycloakService> _logger = logger;

    private readonly IOptions<KeycloakConfiguration> _keycloakSettings = keycloakSettings;

    public readonly static string NO_AUTH_HTTP_CLIENT_NAME = "NoAuthKeycloakClient";
    public readonly static string ADMIN_HTTP_CLIENT_NAME = "AdminKeycloakClient";

    /// <summary>
    /// Получить токен авторизации для пользователя по логину и паролю.
    /// </summary>
    /// <param name="userName">Логин.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>Результат апи с токеном. Null - если данные запроса некорректы или результат неуспешный.</returns>
    /// <remarks>
    /// Документация:
    /// - https://www.keycloak.org/securing-apps/oidc-layers
    /// - https://www.keycloak.org/docs-api/25.0.6/rest-api/#_client_initial_access
    /// </remarks>
    public async Task<AuthorizationTokenDto?> GetUserAuthorizationToken(string userName, string password, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("The beginning of the Keycloak request in the method {methodName}", nameof(GetUserAuthorizationToken));

        const string SCOPE = "openid";
        const string GRAND_TYPE = "password";

        using var client = _httpClientFactory.CreateClient(NO_AUTH_HTTP_CLIENT_NAME);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"realms/{_keycloakSettings.Value.RealmId}/protocol/openid-connect/token")
        {
            Content = new FormUrlEncodedContent([
                new ("client_id", _keycloakSettings.Value.ClientId),
                new ("client_secret", _keycloakSettings.Value.ClientSecret),
                new ("username", userName),
                new ("password", password),
                new ("scope", SCOPE),
                new ("grant_type", GRAND_TYPE),
            ])
        };

        using var result = await client.SendAsync(request, cancellationToken);

        if (!result.IsSuccessStatusCode)
        {
            _logger.LogError("The Keycloak request failed with the code {requestCode} in the method {methodName}",
                result.StatusCode, nameof(GetUserAuthorizationToken));

            return null;
        }

        _logger.LogDebug("The Keycloak request successfully completed with the code {requestCode} in the method {methodName}",
            result.StatusCode, nameof(GetUserAuthorizationToken));

        return await result.Content.ReadFromJsonAsync<AuthorizationTokenDto>(cancellationToken);
    }

    public async Task<AuthorizationTokenDto?> RefreshUserAuthorizationToken(string refreshToken, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("The beginning of the Keycloak request in the method {methodName}", nameof(RefreshUserAuthorizationToken));

        const string SCOPE = "openid";
        const string GRAND_TYPE = "refresh_token";

        using var client = _httpClientFactory.CreateClient(NO_AUTH_HTTP_CLIENT_NAME);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"/realms/{_keycloakSettings.Value.RealmId}/protocol/openid-connect/token")
        {
            Content = new FormUrlEncodedContent([
                new ("client_id", _keycloakSettings.Value.ClientId),
                new ("client_secret", _keycloakSettings.Value.ClientSecret),
                new ("refresh_token", refreshToken),
                new ("scope", SCOPE),
                new ("grant_type", GRAND_TYPE),
            ])
        };

        using var result = await client.SendAsync(request, cancellationToken);

        if (!result.IsSuccessStatusCode)
        {
            _logger.LogError("The Keycloak request failed with the code {requestCode} in the method {methodName}",
                result.StatusCode, nameof(RefreshUserAuthorizationToken));

            return null;
        }

        _logger.LogDebug("The Keycloak request successfully completed with the code {requestCode} in the method {methodName}",
            result.StatusCode, nameof(RefreshUserAuthorizationToken));

        return await result.Content.ReadFromJsonAsync<AuthorizationTokenDto>(cancellationToken);
    }

    /// <summary>
    /// Верифицировать токен (авторизации или обновления).
    /// </summary>
    /// <param name="token">Токен.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>true - если токен еще активен, иначе - false.</returns>
    public async Task<bool> VerificationToken(string token, CancellationToken cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient(NO_AUTH_HTTP_CLIENT_NAME);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"realms/{_keycloakSettings.Value.RealmId}/protocol/openid-connect/token/introspect")
        {
            Content = new FormUrlEncodedContent([
                new ("client_id", _keycloakSettings.Value.ClientId),
                new ("client_secret", _keycloakSettings.Value.ClientSecret),
                new ("token", token),
            ])
        };

        using var result = await client.SendAsync(request, cancellationToken);

        if (!result.IsSuccessStatusCode)
        {
            _logger.LogError("The Keycloak request failed with the code {requestCode} in the method {methodName}",
                result.StatusCode, nameof(VerificationToken));

            return false;
        }

        _logger.LogDebug("The Keycloak request successfully completed with the code {requestCode} in the method {methodName}",
            result.StatusCode, nameof(VerificationToken));

        var response = await result.Content.ReadFromJsonAsync<TokenIntrospectResponseDto>(cancellationToken);

        return response?.IsActive ?? false;
    }
}