using TestTask.Document.Employee.Common.Monads;
using TestTask.Document.Employee.Contract.Dtos;

namespace TestTask.Document.Employee.Contract.Interfaces.DocumentRequest;

/// <summary>
/// Контракт на команды для создания запросов.
/// </summary>
public interface IDocumentRequestCommand
{
    /// <summary>
    /// Создать запрос на получение справки.
    /// </summary>
    /// <param name="requestCommandToCreateDto">Модель создания запроса.</param>
    /// <returns>Идентфикатор запроса.</returns>
    public Task<Result<long, IDictionary<string, string[]>>> CreateDocumentRequest(RequestCommandToCreateDto requestCommandToCreateDto, CancellationToken cancellationToken = default);
}
