using TestTask.Document.Employee.Database.Entities.Enums;

namespace TestTask.Document.Employee.Database.Entities;

/// <summary>
/// Сущность сотрудника.
/// </summary>
public class EmployeeEntity
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Тип сотрудника.
    /// </summary>
    public EmployeeType EmployeeType { get; set; }

    /// <summary>
    /// Должность сотрудника.
    /// </summary>
    public PositionType PositionType { get; set; }
}
