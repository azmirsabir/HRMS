using FluentValidation;

namespace Application.Features.Employee.Command.AddEmployee;

public class AddEmployeeCommandValidator : AbstractValidator<AddEmployeeCommand>
{
    public AddEmployeeCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(50);

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("Invalid department ID.");

        RuleFor(x => x.ServiceInYears)
            .GreaterThanOrEqualTo(0).WithMessage("Service in years cannot be negative.");

        RuleFor(x => x.BaseSalary)
            .GreaterThanOrEqualTo(0).WithMessage("Base salary cannot be negative.");
    }
}