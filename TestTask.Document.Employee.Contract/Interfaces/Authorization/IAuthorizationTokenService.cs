using TestTask.Document.Employee.Common.Monads;
using TestTask.Document.Employee.Contract.Dtos;

namespace TestTask.Document.Employee.Contract.Interfaces.Authorization;

public interface IAuthorizationTokenService
{
    /// <summary>
    /// Название claim по которому в токене лежит идентифкатор пользователя.
    /// </summary>
    static string ApplicationUserIdClaimName => "application_user_id";

    /// <summary>
    /// Получить токен авторизации.
    /// </summary>
    /// <param name="authorizationData">Модель данных авторизации.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Результат с моделью токена.</returns>
    Task<Result<AuthorizationTokenDto?, IDictionary<string, string[]>>> GetAuthorizationToken(UserCredentialsDto authorizationData, CancellationToken cancellationToken);

    /// <summary>
    /// Валидирировать токен авторизации.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>true - если токен прошел верификацию, иначе false. Null - если токен не обнаружен.</returns>
    Task<Result<bool?>> VerificationCurrentUserAccessToken(CancellationToken cancellationToken);
}