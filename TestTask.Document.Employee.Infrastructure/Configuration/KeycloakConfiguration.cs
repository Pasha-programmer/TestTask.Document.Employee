using System.ComponentModel.DataAnnotations;

namespace TestTask.Document.Employee.Infrastructure.Configuration;

public record KeycloakConfiguration
{
    [Required(ErrorMessage = $"{nameof(ServerUrl)} обязателен для заполнения.")]
    public required Uri ServerUrl { get; set; }

    [Required(ErrorMessage = $"{nameof(RealmId)} обязателен для заполнения.")]
    public required string RealmId { get; set; }

    [Required(ErrorMessage = $"{nameof(ClientId)} обязателен для заполнения.")]
    public required string ClientId { get; set; }

    [Required(ErrorMessage = $"{nameof(ClientSecret)} обязателен для заполнения.")]
    public required string ClientSecret { get; set; }
}
