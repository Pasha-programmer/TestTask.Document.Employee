using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestTask.Document.Employee.Common.Monads;
using TestTask.Document.Employee.Contract.Dtos;
using TestTask.Document.Employee.Contract.Dtos.Enums;
using TestTask.Document.Employee.Contract.Dtos.FilterParameters;
using TestTask.Document.Employee.Contract.Interfaces.DocumentProcess;
using TestTask.Document.Employee.Database.Context;
using TestTask.Document.Employee.Database.Entities;

namespace TestTask.Document.Employee.Infrastructure.Services.DocumentProcess;

/// <summary>
/// Реализация контракта на получение информации для обработки запроса.
/// </summary>
internal class DocumentProcessQueryService(
    ILogger<DocumentProcessCommandService> logger,
    IDbContextFactory<EmployeeDocumentDbContext> dbContextFactory
    ) : IDocumentProcessQuery
{
    private readonly ILogger<DocumentProcessCommandService> _logger = logger;
    private readonly IDbContextFactory<EmployeeDocumentDbContext> _dbContextFactory = dbContextFactory;

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyCollection<DocumentRequestFullDto>>> GetDocumentRequestsDetails(
        DocumentRequestFilterParameters? documentRequestFilterParameters,
        CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = from dr in context.DocumentRequests
                    select dr;

        query = ApplyFilterParameters(query, documentRequestFilterParameters);

        var data = await query.Select(dr => new DocumentRequestFullDto
        {
            Id = dr.Id,
            AuthorId = dr.AuthorId,
            DocumentType = (AccountingDocumentType)dr.DocumentType,
            RequestStatus = (RequestStatus)dr.RequestStatus,
            Count = dr.Count,
            Reason = dr.Reason,
            CreateDate = dr.CreateDate,
        }).ToArrayAsync(cancellationToken);

        return Result<IReadOnlyCollection<DocumentRequestFullDto>>.Success(data);
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentRequestFullDto>> GetDocumentRequestDetails(
        long documentRequestId, 
        CancellationToken cancellationToken = default)
    {
        var result = await GetDocumentRequestsDetails(new()
        {
            DocumentRequestIds = [documentRequestId],
        }, cancellationToken);

        if (result.IsFailed || result.Value.Count != 1)
        {
            return Result<DocumentRequestFullDto>.Failed("Не удалось найти запрос.");
        }

        return Result<DocumentRequestFullDto>.Success(result.Value.First());
    }

    /// <summary>
    /// Применить фильтры к запросу.
    /// </summary>
    /// <param name="query">Запрос.</param>
    /// <param name="documentRequestFilterParameters">Параметры фильтрации.</param>
    /// <returns>Запрос.</returns>
    private IQueryable<DocumentRequestEntity> ApplyFilterParameters(
        IQueryable<DocumentRequestEntity> query, 
        DocumentRequestFilterParameters? documentRequestFilterParameters)
    {
        if (documentRequestFilterParameters == null) 
            return query;

        if (documentRequestFilterParameters.DocumentRequestIds?.Count > 0)
            query = query.Where(dr => documentRequestFilterParameters.DocumentRequestIds.Contains(dr.Id));

        if (documentRequestFilterParameters.DocumentRequestAuthorIds?.Count > 0)
            query = query.Where(dr => documentRequestFilterParameters.DocumentRequestAuthorIds.Contains(dr.AuthorId));

        if (documentRequestFilterParameters.DocumentTypes?.Count > 0)
        {
            var castedDocumentTypes = documentRequestFilterParameters.DocumentTypes.Cast<Database.Entities.Enums.AccountingDocumentType>().ToArray();
            query = query.Where(dr => castedDocumentTypes.Contains(dr.DocumentType));
        }

        if (documentRequestFilterParameters.RequestStatuses?.Count > 0)
        {
            var castedRequestStatuses = documentRequestFilterParameters.RequestStatuses.Cast<Database.Entities.Enums.RequestStatus>().ToArray();
            query = query.Where(dr => castedRequestStatuses.Contains(dr.RequestStatus));
        }

        if (documentRequestFilterParameters.MinCount.HasValue)
            query = query.Where(dr => dr.Count >= documentRequestFilterParameters.MinCount);

        if (documentRequestFilterParameters.MaxCount.HasValue)
            query = query.Where(dr => dr.Count < documentRequestFilterParameters.MaxCount);

        if (!string.IsNullOrEmpty(documentRequestFilterParameters.ReasonSubstring))
            query = query.Where(dr => dr.Reason.Contains(documentRequestFilterParameters.ReasonSubstring));

        if (documentRequestFilterParameters.FromCreateDate.HasValue)
            query = query.Where(dr => dr.CreateDate >= documentRequestFilterParameters.FromCreateDate);

        if (documentRequestFilterParameters.ToCreateDate.HasValue)
            query = query.Where(dr => dr.CreateDate < documentRequestFilterParameters.ToCreateDate);

        return query;
    }
}
