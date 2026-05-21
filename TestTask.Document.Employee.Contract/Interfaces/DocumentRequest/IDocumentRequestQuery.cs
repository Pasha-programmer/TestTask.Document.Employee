using TestTask.Document.Employee.Common.Monads;
using TestTask.Document.Employee.Contract.Dtos;

namespace TestTask.Document.Employee.Contract.Interfaces.DocumentRequest;

/// <summary>
/// Контракт на получение информации о запросе.
/// </summary>
public interface IDocumentRequestQuery
{
    /// <summary>
    /// Получить информацию о запросе.
    /// </summary>
    /// <param name="documentRequestFilterParameters">Параметры фильтрации.</param>
    /// <returns>Коллекция моделей запроса.</returns>
    public Task<Result<IReadOnlyCollection<DocumentRequestDto>>> GetDocumentRequests(DocumentRequestFilterParameters documentRequestFilterParameters);
}
