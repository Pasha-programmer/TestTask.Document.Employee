using System.ComponentModel;

namespace TestTask.Document.Employee.Contract.Dtos.Enums;

/// <summary>
/// Статуса запроса.
/// </summary>
public enum RequestStatus : int
{
    /// <summary>
    /// Запрос отправлен.
    /// </summary>
    [Description("Запрос отправлен")]
    Sent = 0,

    /// <summary>
    /// Запрос в обработке.
    /// </summary>
    [Description("Запрос в обработке")]
    InProcess = 1,

    /// <summary>
    /// В запросе отказано.
    /// </summary>
    [Description("В запросе отказано")]
    Refusal = 2,

    /// <summary>
    /// Запрос обработан.
    /// </summary>
    [Description("Запрос обработан")]
    Processed = 3,
}
