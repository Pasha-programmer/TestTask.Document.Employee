using TestTask.Document.Employee.Common.Monads;
using TestTask.Document.Employee.Contract.Dtos;

namespace TestTask.Document.Employee.Contract.Interfaces.DocumentProcess;

/// <summary>
/// Контракт на получение информации для обработки запроса.
/// </summary>
public interface IDocumentProcessQuery
{
    /// <summary>
    /// Получить детали запроса.
    /// </summary>
    /// <param name="documentRequestFilterParameters"></param>
    /// <returns></returns>
    public Task<Result<DocumentRequestDto>> GetDocumentRequestsDetails(DocumentRequestFilterParameters documentRequestFilterParameters);
}
