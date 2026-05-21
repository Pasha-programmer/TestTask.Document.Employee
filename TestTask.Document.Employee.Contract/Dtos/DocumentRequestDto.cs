using TestTask.Document.Employee.Contract.Dtos.Enums;

namespace TestTask.Document.Employee.Contract.Dtos;

/// <summary>
/// Полная модель запроса на получение справки.
/// </summary>
public record DocumentRequestDto
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public required long Id { get; set; }

    /// <summary>
    /// Идентификатор автора запроса.
    /// </summary>
    public required long AuthorId { get; set; }

    /// <summary>
    /// Тип запрашиваемого бухгатерского документа.
    /// </summary>
    public required AccountingDocumentType DocumentType { get; set; }

    /// <summary>
    /// Статус запроса.
    /// </summary>
    public required RequestStatus RequestStatus { get; set; }

    /// <summary>
    /// Количество экземпляров.
    /// </summary>
    public required int Count { get; set; }

    /// <summary>
    /// Причина запроса.
    /// </summary>
    public required string Reason { get; set; }
}
