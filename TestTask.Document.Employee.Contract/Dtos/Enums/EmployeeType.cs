using System.ComponentModel;

namespace TestTask.Document.Employee.Contract.Dtos.Enums;

/// <summary>
/// Тип сотрудника.
/// </summary>
public enum EmployeeType : int
{
    /// <summary>
    /// Сотрудник.
    /// </summary>
    [Description("Сотрудник")]
    Employee = 1,
}
