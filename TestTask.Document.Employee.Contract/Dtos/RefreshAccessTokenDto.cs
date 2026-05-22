namespace TestTask.Document.Employee.Contract.Dtos;

/// <summary>
/// Модель обноваления токена доступа.
/// </summary>
public record RefreshAccessTokenDto
{
    /// <summary>
    /// Значение токена.
    /// </summary>
    public required string RefreshToken { get; set; }
}
