using System.ComponentModel.DataAnnotations;

namespace TestTask.Document.Employee.Infrastructure.Configuration;

public record AdminCredentialsConfiguration
{
    [Required(ErrorMessage = $"{nameof(Login)} обязателен для заполнения.")]
    public required string Login { get; set; }

    [Required(ErrorMessage = $"{nameof(Password)} обязателен для заполнения.")]
    public required string Password { get; set; }
}
