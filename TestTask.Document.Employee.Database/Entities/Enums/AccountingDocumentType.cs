using System.ComponentModel;

namespace TestTask.Document.Employee.Database.Entities.Enums;

/// <summary>
/// Типы бухгатерских документов.
/// </summary>
public enum AccountingDocumentType : int
{
    /// <summary>
    /// 2-НДФЛ.
    /// </summary>
    [Description("2-НДФЛ")]
    PersonalIncomeTax = 1,

    /// <summary>
    /// Место работы и стаж.
    /// </summary>
    [Description("Место работы и стаж")]
    WorkPlaceAndSeniority = 2,

    /// <summary>
    /// Средний заработок.
    /// </summary>
    [Description("Средний заработок")]
    AverageEarnings = 3,

    /// <summary>
    /// Произвольный тип.
    /// </summary>
    [Description("Произвольный тип")]
    Other = 4,
}
