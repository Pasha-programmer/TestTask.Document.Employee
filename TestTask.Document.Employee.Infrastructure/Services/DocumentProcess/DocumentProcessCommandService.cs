using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestTask.Document.Employee.Common.Monads;
using TestTask.Document.Employee.Contract.Dtos;
using TestTask.Document.Employee.Contract.Interfaces.DocumentProcess;
using TestTask.Document.Employee.Database.Context;
using TestTask.Document.Employee.Database.Entities;

namespace TestTask.Document.Employee.Infrastructure.Services.DocumentProcess;

/// <summary>
/// Реализация контракта на команды для обработки запроса.
/// </summary>
internal class DocumentProcessCommandService(
    ILogger<DocumentProcessCommandService> logger,
    IDbContextFactory<EmployeeDocumentDbContext> dbContextFactory
    ) : IDocumentProcessCommand
{
    private readonly ILogger<DocumentProcessCommandService> _logger = logger;
    private readonly IDbContextFactory<EmployeeDocumentDbContext> _dbContextFactory = dbContextFactory;

    /// <inheritdoc/>
    public async Task<Result<IDictionary<string, string[]>?>> UpdateStatusDocumentRequest(
        RequestCommandToUpdateDto requestCommandToUpdateDto,
        CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var documentRequestEntity = new DocumentRequestEntity
        {
            Id = requestCommandToUpdateDto.Id,
            RequestStatus = (Database.Entities.Enums.RequestStatus)requestCommandToUpdateDto.RequestStatus,
        };

        context.Attach(documentRequestEntity);
        context.Entry(documentRequestEntity).Property(x => x.RequestStatus).IsModified = true;

        if ((await context.SaveChangesAsync(cancellationToken)) > 0)
        {
            _logger.LogInformation("Статус запроса #{DocumentRequestId} обновлен", requestCommandToUpdateDto.Id);
            return Result<IDictionary<string, string[]>?>.Success(null);
        }

        _logger.LogError("Ошибка обновления статуса запроса #{DocumentRequestId}", requestCommandToUpdateDto.Id);
        return Result<IDictionary<string, string[]>?>.Failed("Ошибка обновления статуса запроса");
    }
}
