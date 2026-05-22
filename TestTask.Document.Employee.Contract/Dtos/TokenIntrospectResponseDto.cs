using System.Text.Json.Serialization;

namespace RadioJune.IdentityConnector.Contract.Dtos;

public class TokenIntrospectResponseDto
{
    [JsonPropertyName("active")]
    public bool IsActive { get; set; }
}
