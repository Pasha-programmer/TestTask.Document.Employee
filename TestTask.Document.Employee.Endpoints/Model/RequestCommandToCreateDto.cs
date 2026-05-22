using TestTask.Document.Employee.Contract.Dtos.Enums;

namespace TestTask.Document.Employee.Endpoints.Model;

/// <summary>
/// Модель команды для создания запроса на получение справки.
/// </summary>
public record RequestCommandToCreateDto
{
    /// <summary>
    /// Тип запрашиваемого бухгатерского документа.
    /// </summary>
    public required AccountingDocumentType DocumentType { get; set; }

    /// <summary>
    /// Количество экземпляров.
    /// </summary>
    public required int Count { get; set; }

    /// <summary>
    /// Причина запроса.
    /// </summary>
    public required string Reason { get; set; }
}
