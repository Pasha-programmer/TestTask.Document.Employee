using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using TestTask.Document.Employee.Common.Monads;
using TestTask.Document.Employee.Contract.Dtos;
using TestTask.Document.Employee.Contract.Interfaces.DocumentRequest;
using TestTask.Document.Employee.Database.Context;
using TestTask.Document.Employee.Database.Entities;
using TestTask.Document.Employee.Infrastructure.Services.DocumentProcess;

namespace TestTask.Document.Employee.Infrastructure.Services.DocumentRequest;

/// <summary>
/// Реализация контракта на команды для создания запросов.
/// </summary>
internal class DocumentRequestCommandService(
    ILogger<DocumentProcessCommandService> logger,
    IDbContextFactory<EmployeeDocumentDbContext> dbContextFactory
    ) : IDocumentRequestCommand
{
    private readonly ILogger<DocumentProcessCommandService> _logger = logger;
    private readonly IDbContextFactory<EmployeeDocumentDbContext> _dbContextFactory = dbContextFactory;

    /// <inheritdoc/>
    public async Task<Result<long, IDictionary<string, string[]>>> CreateDocumentRequest(RequestCommandToCreateDto requestCommandToCreateDto)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var documentRequestEntity = new DocumentRequestEntity
        {
            AuthorId = requestCommandToCreateDto.AuthorId,
            DocumentType = (Database.Entities.Enums.AccountingDocumentType)requestCommandToCreateDto.DocumentType,
            RequestStatus = (Database.Entities.Enums.RequestStatus)requestCommandToCreateDto.RequestStatus,
            Count = requestCommandToCreateDto.Count,
            Reason = requestCommandToCreateDto.Reason,
            CreateDate = DateTimeOffset.Now,
        };

        context.Attach(documentRequestEntity);
        context.Entry(documentRequestEntity).Property(x => x.RequestStatus).IsModified = true;

        if ((await context.SaveChangesAsync()) > 0)
        {
            _logger.LogInformation("Запрос #{DocumentRequestId} успешно создан", documentRequestEntity.Id);
            return Result<long, IDictionary<string, string[]>>.Success(documentRequestEntity.Id);
        }

        _logger.LogError("Ошибка создания запроса");
        return Result<long, IDictionary<string, string[]>>.Failed("Ошибка создания запроса");
    }
}
