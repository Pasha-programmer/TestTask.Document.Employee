using System.ComponentModel.DataAnnotations;

namespace TestTask.Document.Employee.Infrastructure.Configuration;

public record AuthSettings
{
    [Required(ErrorMessage = $"{nameof(Issuer)} обязателен для заполнения.")]
    public required string Issuer { get; set; }

    [Required(ErrorMessage = $"{nameof(Audience)} обязателен для заполнения.")]
    public required string Audience { get; set; }

    [Required(ErrorMessage = $"{nameof(SecretKey)} обязателен для заполнения.")]
    public required string SecretKey { get; set; }
}
