using Application.Features.Employee.Command.UpdateEmployee;
using FluentValidation;

namespace Application.Features.Employee.Command.UpdateEmployee;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.ServiceInYears)
            .GreaterThanOrEqualTo(0).WithMessage("Service in years cannot be negative.");
        
        RuleFor(x => x.Degree)
            .IsInEnum()
            .WithMessage("Invalid degree level.");


        RuleFor(x => x.BaseSalary)
            .GreaterThanOrEqualTo(0).WithMessage("Base salary cannot be negative.");
    }
}