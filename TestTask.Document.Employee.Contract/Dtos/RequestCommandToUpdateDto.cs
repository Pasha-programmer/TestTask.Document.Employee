using TestTask.Document.Employee.Contract.Dtos.Enums;

namespace TestTask.Document.Employee.Contract.Dtos;

/// <summary>
/// Модель команды для обновления статуса запроса на получение справки.
/// </summary>
public record RequestCommandToUpdateDto
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public required long Id { get; set; }

    /// <summary>
    /// Статус запроса.
    /// </summary>
    public required RequestStatus RequestStatus { get; set; }
}
