using FluentValidation;
using TestTask.Document.Employee.Contract.Dtos;

namespace TestTask.Document.Employee.Contract.Validators;

public class RequestCommandToCreateValidator : AbstractValidator<RequestCommandToCreateDto>
{
    public RequestCommandToCreateValidator()
    {
        RuleFor(x => x.DocumentType)
            .NotEmpty()
            .WithMessage("Тип запрашиваемого бухгатерского документа обзателен к заполнению");

        RuleFor(x => x.Count)
            .NotEmpty()
            .WithMessage("Количество экземпляров обзателено к заполнению")
            .GreaterThan(0)
            .WithMessage("Количество экземпляров должно быть больше 0");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Причина обязательна к заполнению");
    }
}
