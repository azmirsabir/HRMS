using FluentValidation;

namespace Application.Features.Department.Command.AddDepartment;

public class AddDepartmentCommandValidator : AbstractValidator<AddDepartmentCommand>
{
    public AddDepartmentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");
    }
}