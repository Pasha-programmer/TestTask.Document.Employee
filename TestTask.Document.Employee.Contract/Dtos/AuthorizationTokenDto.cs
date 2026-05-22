using System.Text.Json.Serialization;

namespace TestTask.Document.Employee.Contract.Dtos;

/// <summary>
/// Токен авторизации. 
/// </summary>
public record AuthorizationTokenDto
{
    /// <summary>
    /// Токен доступа.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// Токен обновления токена доступа.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}