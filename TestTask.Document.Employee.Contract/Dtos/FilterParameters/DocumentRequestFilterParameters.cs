using TestTask.Document.Employee.Contract.Dtos.Enums;

namespace TestTask.Document.Employee.Contract.Dtos.FilterParameters;

/// <summary>
/// Параметры фильтрации запросов на получение справок.
/// </summary>
public record DocumentRequestFilterParameters
{
    /// <summary>
    /// Идентификаторы запросов.
    /// </summary>
    public IReadOnlyCollection<long>? DocumentRequestIds { get; set; }

    /// <summary>
    /// Идентификаторы авторов запросов.
    /// </summary>
    public IReadOnlyCollection<long>? DocumentRequestAuthorIds { get; set; }

    /// <summary>
    /// Типs запрашиваемых бухгатерских документов.
    /// </summary>
    public IReadOnlyCollection<AccountingDocumentType>? DocumentTypes { get; set; }

    /// <summary>
    /// Статусы запросов.
    /// </summary>
    public IReadOnlyCollection<RequestStatus>? RequestStatuses { get; set; }

    /// <summary>
    /// Минимальное количество экземпляров.
    /// </summary>
    public int? MinCount { get; set; }

    /// <summary>
    /// Максимальное количество экземпляров.
    /// </summary>
    public int? MaxCount { get; set; }

    /// <summary>
    /// Причина запроса.
    /// </summary>
    public string? ReasonSubstring { get; set; }

    /// <summary>
    /// Начальная дата создания запроса.
    /// </summary>
    public DateTimeOffset? FromCreateDate { get; set; }

    /// <summary>
    /// Конечная дата создания запроса.
    /// </summary>
    public DateTimeOffset? ToCreateDate { get; set; }
}
