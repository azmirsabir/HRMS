using FluentValidation;

namespace Application.Features.Leave.Command.AddLeave;

public class AddLeaveCommandValidator : AbstractValidator<AddLeaveCommand>
{
    public AddLeaveCommandValidator()
    {

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be the same or after start date.");

        RuleFor(x => x.Reason)
            .MaximumLength(250).WithMessage("Reason cannot exceed 250 characters.");
    }
}