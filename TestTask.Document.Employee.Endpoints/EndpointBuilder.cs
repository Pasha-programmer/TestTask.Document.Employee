using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using TestTask.Document.Employee.Common.Monads;

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
        async ([FromServices] ILoggerFactory loggerFactory
        
        ) =>
           {
               var result = new Result<int, IDictionary<string, string[]>>();

               if (result.IsFailed)
                   return result.Error == null
                       ? Results.Problem(result.Details)
                       : Results.ValidationProblem(result.Error, result.Details);

               return Results.Ok(result.Value);
           })
           .WithSummary("Отправить запрос на получение справки")
           .WithDescription("Создает запрос на получение справки")
           .Produces<AddAudioRecordFileResponseDto>(StatusCodes.Status200OK)
           .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        employeeDocumentGroupEndpoints.MapGet("",
        async ([FromServices] ILoggerFactory loggerFactory

        ) =>
        {
            var result = new Result<int, IDictionary<string, string[]>>();

            if (result.IsFailed)
                return result.Error == null
                    ? Results.Problem(result.Details)
                    : Results.ValidationProblem(result.Error, result.Details);

            return Results.Ok(result.Value);
        })
           .WithSummary("Получить информацию о запросах на получение справок")
           .WithDescription("Возвращает информацию о запросах на получение справок")
           .Produces<AddAudioRecordFileResponseDto>(StatusCodes.Status200OK)
           .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        employeeDocumentGroupEndpoints.MapGet("/document",
        async ([FromServices] ILoggerFactory loggerFactory

        ) =>
        {
            var result = new Result<int, IDictionary<string, string[]>>();

            if (result.IsFailed)
                return result.Error == null
                    ? Results.Problem(result.Details)
                    : Results.ValidationProblem(result.Error, result.Details);

            return Results.Ok(result.Value);
        })
           .WithSummary("Получить детали запроса на получение справок")
           .WithDescription("Возвращает детали запроса на получение справок")
           .Produces<AddAudioRecordFileResponseDto>(StatusCodes.Status200OK)
           .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        employeeDocumentGroupEndpoints.MapPut("/requested-document",
        async ([FromServices] ILoggerFactory loggerFactory

        ) =>
        {
            var result = new Result<int, IDictionary<string, string[]>>();

            if (result.IsFailed)
                return result.Error == null
                    ? Results.Problem(result.Details)
                    : Results.ValidationProblem(result.Error, result.Details);

            return Results.Ok(result.Value);
        })
           .WithSummary("Обновить детали запроса на получение справок")
           .WithDescription("Обновленные детали запроса на получение справок")
           .Produces<AddAudioRecordFileResponseDto>(StatusCodes.Status200OK)
           .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }
}
