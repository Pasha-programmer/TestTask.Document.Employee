using TestTask.Document.Employee.Contract.Dtos;

namespace TestTask.Document.Employee.Contract.Interfaces.IdentityConnector;

/// <summary>
/// Контракт доступа.
/// </summary>
public interface IIdentityConnectorService
{
    /// <summary>
    /// Получить токен авторизации для пользователя по логину и паролю.
    /// </summary>
    /// <param name="userName">Логин.</param>
    /// <param name="password">Пароль.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Результат апи с токеном. Null - если данные запроса некорректы или результат неуспешный.</returns>
    Task<AuthorizationTokenDto?> GetUserAuthorizationToken(string userName, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить токен авторизации для пользователя токену обновления.
    /// </summary>
    /// <param name="refreshToken">Токен обновления.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Результат апи с токеном. Null - если данные запроса некорректы или результат неуспешный.</returns>
    Task<AuthorizationTokenDto?> RefreshUserAuthorizationToken(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Верифицировать токен (авторизации или обновления).
    /// </summary>
    /// <param name="token">Токен.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>true - если токен еще активен, иначе - false.</returns>
    Task<bool> VerificationToken(string token, CancellationToken cancellationToken = default);
}

