using TestTask.Document.Employee.Contract.Dtos.Enums;

namespace TestTask.Document.Employee.Contract.Dtos;

/// <summary>
/// Полная модель запроса на получение справки.
/// </summary>
public record DocumentRequestFullDto : DocumentRequestDto
{
    /// <summary>
    /// Количество экземпляров.
    /// </summary>
    public required int Count { get; set; }

    /// <summary>
    /// Причина запроса.
    /// </summary>
    public required string Reason { get; set; }
}
