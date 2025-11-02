using Core.Domain.Leave;
using FluentValidation;

namespace Application.Features.Leave.Command.UpdateLeave;

public class UpdateLeaveCommandValidator : AbstractValidator<UpdateLeaveCommand>
{
    public UpdateLeaveCommandValidator()
    {
        RuleFor(x => x.Status)
            .Must(s => s == LeaveStatus.Accepted || s == LeaveStatus.Rejected)
            .WithMessage("Invalid leave status.");

        When(x => x.Status == LeaveStatus.Rejected, () =>
        {
            RuleFor(x => x.RejectionReason)
                .NotEmpty()
                .WithMessage("Rejection reason is required when the leave is rejected.")
                .MaximumLength(100)
                .WithMessage("Rejection reason cannot exceed 100 characters.");
        });
    }
}