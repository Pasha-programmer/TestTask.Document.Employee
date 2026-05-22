using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestTask.Document.Employee.Common.Monads;
using TestTask.Document.Employee.Contract.Dtos;
using TestTask.Document.Employee.Contract.Dtos.Enums;
using TestTask.Document.Employee.Contract.Dtos.FilterParameters;
using TestTask.Document.Employee.Contract.Interfaces.DocumentRequest;
using TestTask.Document.Employee.Database.Context;
using TestTask.Document.Employee.Database.Entities;
using TestTask.Document.Employee.Infrastructure.Services.DocumentProcess;

namespace TestTask.Document.Employee.Infrastructure.Services.DocumentRequest;

internal class DocumentRequestQueryService(
    ILogger<DocumentProcessCommandService> logger,
    IDbContextFactory<EmployeeDocumentDbContext> dbContextFactory
    ) : IDocumentRequestQuery
{
    private readonly ILogger<DocumentProcessCommandService> _logger = logger;
    private readonly IDbContextFactory<EmployeeDocumentDbContext> _dbContextFactory = dbContextFactory;

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyCollection<DocumentRequestDto>>> GetDocumentRequests(
        DocumentRequestFilterParameters documentRequestFilterParameters,
        CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = from dr in context.DocumentRequests
                    select dr;

        query = ApplyFilterParameters(query, documentRequestFilterParameters);

        var data = await query.Select(dr => new DocumentRequestDto
        {
            Id = dr.Id,
            Author = dr.Author,
            DocumentType = (AccountingDocumentType)dr.DocumentType,
            RequestStatus = (RequestStatus)dr.RequestStatus,
            CreateDate = dr.CreateDate,
        }).ToArrayAsync(cancellationToken);

        return Result<IReadOnlyCollection<DocumentRequestDto>>.Success(data);
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

        if (documentRequestFilterParameters.DocumentRequestIds?.Length > 0)
            query = query.Where(dr => documentRequestFilterParameters.DocumentRequestIds.Contains(dr.Id));

        if (documentRequestFilterParameters.DocumentRequestAuthors?.Length > 0)
            query = query.Where(dr => documentRequestFilterParameters.DocumentRequestAuthors.Contains(dr.Author));

        if (documentRequestFilterParameters.DocumentTypes?.Length > 0)
        {
            var castedDocumentTypes = documentRequestFilterParameters.DocumentTypes.Cast<Database.Entities.Enums.AccountingDocumentType>().ToArray();
            query = query.Where(dr => castedDocumentTypes.Contains(dr.DocumentType));
        }

        if (documentRequestFilterParameters.RequestStatuses?.Length > 0)
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
