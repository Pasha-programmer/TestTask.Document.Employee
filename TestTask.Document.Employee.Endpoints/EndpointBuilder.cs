using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using TestTask.Document.Employee.Contract.Dtos;
using TestTask.Document.Employee.Contract.Dtos.Enums;
using TestTask.Document.Employee.Contract.Dtos.FilterParameters;
using TestTask.Document.Employee.Contract.Interfaces.Authorization;
using TestTask.Document.Employee.Contract.Interfaces.DocumentProcess;
using TestTask.Document.Employee.Contract.Interfaces.DocumentRequest;

namespace TestTask.Document.Employee.Endpoints;

public static class EndpointBuilder
{
    public const string VERSION = "v1.0";

    /// <summary>
    /// Использовать end points.
    /// </summary>
    /// <param name="endpoints">Строитель маршрутов в приложении.</param>
    public static void UseEmployeeDocumentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/api/health");

        var employeeDocumentGroupEndpoints = endpoints.MapGroup($"/api/{VERSION}/document")
            .WithTags("Документы сотрудника")
            .RequireAuthorization();

        employeeDocumentGroupEndpoints.MapPost("/send-request",
        async (
            [FromServices] IDocumentRequestCommand documentRequestCommand,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromBody] Model.RequestCommandToCreateDto requestCommandToCreateDto,
            CancellationToken cancellationToken = default
        ) =>
            {
                var roleClaim = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!;

                if (roleClaim.Value != PositionType.Other.ToString())
                {
                    return Results.Forbid();
                }

                var nameClaim = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Name)!;

                var result = await documentRequestCommand.CreateDocumentRequest(new()
                {
                    Author = nameClaim.Value,
                    DocumentType = requestCommandToCreateDto.DocumentType,
                    Count = requestCommandToCreateDto.Count,
                    Reason = requestCommandToCreateDto.Reason,
                }, cancellationToken);

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
            [FromRoute] int id,
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

        employeeDocumentGroupEndpoints.MapPut("/{id:int}/status",
        async (
            [FromServices] IDocumentProcessCommand documentProcessCommand,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromRoute] int id,
            [FromBody] RequestStatus requestStatus,
            CancellationToken cancellationToken = default
        ) =>
        {
            var roleClaim = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role);

            if (roleClaim.Value != PositionType.Accountant.ToString())
            {
                return Results.Forbid();
            }

            var result = await documentProcessCommand.UpdateStatusDocumentRequest(new()
            {
                Id = id,
                RequestStatus = requestStatus,
            }, cancellationToken);

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

        var usersMapGroup = endpoints.MapGroup($"/api/{VERSION}/users")
            .WithTags("Авторизация/Регистрация/Пользователи");

        usersMapGroup.MapPost("/token", async (
            [FromServices] IAuthorizationTokenService service,
            [FromBody] UserCredentialsDto model,
            CancellationToken cancellationToken) =>
        {
            var result = await service.GetAuthorizationToken(model, cancellationToken);

            if (result.IsFailed)
                return result.Error == null
                    ? Results.Problem(result.Details, statusCode: StatusCodes.Status401Unauthorized)
                    : Results.ValidationProblem(result.Error, result.Details);

            return Results.Ok(result.Value);
        })
        .WithSummary("Получить токен авторизации")
        .WithDescription("Получает токен авторизации")
        .Produces<AuthorizationTokenDto>(StatusCodes.Status200OK)
        .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized)
        .AllowAnonymous();
    }
}
