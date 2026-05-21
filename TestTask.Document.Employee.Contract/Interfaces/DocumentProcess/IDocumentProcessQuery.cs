using TestTask.Document.Employee.Common.Monads;
using TestTask.Document.Employee.Contract.Dtos;
using TestTask.Document.Employee.Contract.Dtos.FilterParameters;

namespace TestTask.Document.Employee.Contract.Interfaces.DocumentProcess;

/// <summary>
/// Контракт на получение информации для обработки запроса.
/// </summary>
public interface IDocumentProcessQuery
{
    /// <summary>
    /// Получить детали запроса.
    /// </summary>
    /// <param name="documentRequestFilterParameters">Параметры фильтрации.</param>
    /// <returns>Модели запросов.</returns>
    public Task<Result<IReadOnlyCollection<DocumentRequestFullDto>>> GetDocumentRequestsDetails(
        DocumentRequestFilterParameters? documentRequestFilterParameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить детали запроса.
    /// </summary>
    /// <param name="documentRequestId">Идентификатор запроса.</param>
    /// <returns>Модель запроса.</returns>
    public Task<Result<DocumentRequestFullDto>> GetDocumentRequestDetails(
        long documentRequestId,
        CancellationToken cancellationToken = default);
}
