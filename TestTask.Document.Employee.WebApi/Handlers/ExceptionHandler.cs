using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TestTask.Document.Employee.WebApi.Handlers;

/// <summary>
/// Слушатель исключений
/// </summary>
internal class ExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Попытаться обработать ошибку. Добавить в http response детали ошибки (problem details).
    /// </summary>
    /// <param name="httpContext">HTTP контекст.</param>
    /// <param name="exception">Прослушанное исключение.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>true.</returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            BadHttpRequestException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = status;

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = "Что-то пошло не так.",
            Type = exception.GetType().Name,
            Detail = exception.Message
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
