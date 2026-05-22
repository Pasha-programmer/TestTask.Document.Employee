namespace TestTask.Document.Employee.Contract.Dtos;

/// <summary>
/// Модель данных авторизации.
/// </summary>
public record UserCredentialsDto
{
    /// <summary>
    /// Логин.
    /// </summary>
    public required string Login { get; set; }

    /// <summary>
    /// Пароль.
    /// </summary>
    public required string Password { get; set; }
}
