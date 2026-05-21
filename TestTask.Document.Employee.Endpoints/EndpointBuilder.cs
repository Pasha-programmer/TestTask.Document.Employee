using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TestTask.Document.Employee.Contract.Dtos;
using TestTask.Document.Employee.Contract.Dtos.FilterParameters;
using TestTask.Document.Employee.Contract.Interfaces.DocumentProcess;
using TestTask.Document.Employee.Contract.Interfaces.DocumentRequest;

namespace TestTask.Document.Employee.Endpoints;

public static class EndpointBuilder
{
    private const string LOGGER_NAME = "EmployeeDocumentEndpoints";
    public const string VERSION = "v1.0";

    /// <summary>
    /// Использовать end points.
    /// </summary>
    /// <param name="endpoints">Строитель маршрутов в приложении.</param>
    public static void UseEmployeeDocumentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var employeeDocumentGroupEndpoints = endpoints.MapGroup($"/api/{VERSION}/document/employee")
            .WithTags("Документы сотрудника")
            .RequireAuthorization();

        employeeDocumentGroupEndpoints.MapPost("/send-request",
        async (
            [FromServices] IDocumentRequestCommand documentRequestCommand,
            [FromBody] RequestCommandToCreateDto requestCommandToCreateDto,
            CancellationToken cancellationToken = default
        ) =>
           {
               var result = await documentRequestCommand.CreateDocumentRequest(requestCommandToCreateDto, cancellationToken);

               if (result.IsFailed)
                   return result.Error == null
                       ? Results.Problem(result.Details)
                       : Results.ValidationProblem(result.Error, result.Details);

               return Results.Ok(result.Value);
           })
           .WithSummary("Отправить запрос на получение справки")
           .WithDescription("Создает запрос на получение справки")
           .Produces(StatusCodes.Status200OK)
           .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        employeeDocumentGroupEndpoints.MapGet("",
        async (
            [FromServices] IDocumentRequestQuery documentRequestQuery,
            [AsParameters] DocumentRequestFilterParameters documentRequestFilterParameters,
            CancellationToken cancellationToken = default
        ) =>
        {
            var result = await documentRequestQuery.GetDocumentRequests(documentRequestFilterParameters, cancellationToken);

            if (result.IsSuccess && result.Value == null)
                return Results.NotFound(result.Details);

            if (result.IsFailed)
                return Results.Problem(result.Details);

            return Results.Ok(result.Value);
        })
           .WithSummary("Получить информацию о запросах на получение справок")
           .WithDescription("Возвращает информацию о запросах на получение справок")
           .Produces<DocumentRequestDto>(StatusCodes.Status200OK)
           .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        employeeDocumentGroupEndpoints.MapGet("/details",
        async (
            [FromServices] IDocumentProcessQuery documentProcessQuery,
            [AsParameters] DocumentRequestFilterParameters documentRequestFilterParameters,
            CancellationToken cancellationToken = default
        ) =>
        {
            var result = await documentProcessQuery.GetDocumentRequestsDetails(documentRequestFilterParameters, cancellationToken);

            if (result.IsSuccess && result.Value == null)
                return Results.NotFound(result.Details);

            if (result.IsFailed)
                return Results.Problem(result.Details);

            return Results.Ok(result.Value);
        })
           .WithSummary("Получить детали запросов на получение справок")
           .WithDescription("Возвращает детали запросов на получение справок")
           .Produces<DocumentRequestFullDto>(StatusCodes.Status200OK)
           .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        employeeDocumentGroupEndpoints.MapGet("/details/{id:int}",
        async (
            [FromServices] IDocumentProcessQuery documentProcessQuery,
            int id,
            CancellationToken cancellationToken = default
        ) =>
        {
            var result = await documentProcessQuery.GetDocumentRequestDetails(id, cancellationToken);

            if (result.IsSuccess && result.Value == null)
                return Results.NotFound(result.Details);

            if (result.IsFailed)
                return Results.Problem(result.Details);

            return Results.Ok(result.Value);
        })
           .WithSummary("Получить детали запроса на получение справок")
           .WithDescription("Возвращает детали запроса на получение справок")
           .Produces<DocumentRequestFullDto>(StatusCodes.Status200OK)
           .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        employeeDocumentGroupEndpoints.MapPut("/requested-document",
        async (
            [FromServices] IDocumentProcessCommand documentProcessCommand,
            [FromBody] RequestCommandToUpdateDto requestCommandToUpdateDto,
            CancellationToken cancellationToken = default
        ) =>
        {
            var result = await documentProcessCommand.UpdateStatusDocumentRequest(requestCommandToUpdateDto, cancellationToken);

            if (result.IsFailed)
                return result.Value == null
                        ? Results.Problem(result.Details)
                        : Results.ValidationProblem(result.Value, result.Details);

            return Results.Ok(result.Value);
        })
           .WithSummary("Обновить детали запроса на получение справок")
           .WithDescription("Обновленные детали запроса на получение справок")
           .Produces(StatusCodes.Status200OK)
           .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }
}
