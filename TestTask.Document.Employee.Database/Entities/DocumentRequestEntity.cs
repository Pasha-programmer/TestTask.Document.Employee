using TestTask.Document.Employee.Database.Entities.Enums;

namespace TestTask.Document.Employee.Database.Entities;

/// <summary>
/// Сущность запроса на получение справки.
/// </summary>
public class DocumentRequestEntity
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Идентификатор автора запроса.
    /// </summary>
    public long AuthorId { get; set; }

    /// <summary>
    /// Тип запрашиваемого бухгатерского документа.
    /// </summary>
    public AccountingDocumentType DocumentType { get; set; }

    /// <summary>
    /// Статус запроса.
    /// </summary>
    public RequestStatus RequestStatus { get; set; }

    /// <summary>
    /// Количество экземпляров.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Причина запроса.
    /// </summary>
    public string Reason { get; set; }

    /// <summary>
    /// Дата создания запроса.
    /// </summary>
    public DateTimeOffset CreateDate { get; set; }
}
