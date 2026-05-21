using TestTask.Document.Employee.Common.Monads;
using TestTask.Document.Employee.Contract.Dtos;

namespace TestTask.Document.Employee.Contract.Interfaces.DocumentProcess;

/// <summary>
/// Контракт на команды для обработки запроса.
/// </summary>
public interface IDocumentProcessCommand
{
    /// <summary>
    /// Обновить статус запроса.
    /// </summary>
    /// <param name="requestCommandToUpdateDto">Модель команды для обновления статуса</param>
    public Task<Result<IDictionary<string, string[]>?>> UpdateStatusDocumentRequest(RequestCommandToUpdateDto requestCommandToUpdateDto, CancellationToken cancellationToken = default);
}
