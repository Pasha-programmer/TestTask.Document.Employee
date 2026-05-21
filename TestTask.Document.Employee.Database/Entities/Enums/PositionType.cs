using System.ComponentModel;

namespace TestTask.Document.Employee.Database.Entities.Enums;

/// <summary>
/// Тип сотрудника.
/// </summary>
public enum PositionType : int
{
    /// <summary>
    /// Не обозначенный сотрудник.
    /// </summary>
    [Description("Не обозначен")]
    Other = 0,

    /// <summary>
    /// Бухгалтер.
    /// </summary>
    [Description("Бухгалтер")]
    Accountant = 1,
}
