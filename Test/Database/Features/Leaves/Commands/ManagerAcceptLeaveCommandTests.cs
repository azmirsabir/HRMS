using Application.Features.Leave.Command.AddLeave;
using Application.Features.Leave.Command.UpdateLeave;
using Application.Interfaces;
using Core.Domain.Leave;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Test.Database.Seeds;

namespace Test.Database.Features.Leaves.Commands;

public class ManagerAcceptLeaveCommandTests
{
    [Fact]
    public async Task Handle_Manager_Accepts_First_Pending_Leave()
    {
        var (context, aliUser, aliEmployee, managerUser, managerEmployee, auditorEmployee, auditorUser)
            = await DataSeed.SeedTestDataAsync();

        // Ali creates a leave request
        var currentUserMock = new Mock<ICurrentUser>();
        currentUserMock.Setup(c => c.Id).Returns(aliUser.Id);
        var addHandler = new AddLeaveCommandHandler(context, currentUserMock.Object);
        var command = new AddLeaveCommand
        {
            LeaveType = LeaveType.Annual,
            Reason = "Vacation",
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 1, 5)
        };
        await addHandler.Handle(command, CancellationToken.None);

        // Manager accepts the leave
        var currentManagerMock = new Mock<ICurrentUser>();
        currentManagerMock.Setup(c => c.Id).Returns(managerUser.Id);
        var updateHandler = new UpdateLeaveCommandHandler(currentManagerMock.Object, context);

        var leaveEntity = await context.Leaves.FirstOrDefaultAsync(l => l.Status == LeaveStatus.Pending);
        leaveEntity.Should().NotBeNull();

        var updateCommand = new UpdateLeaveCommand
        {
            Id = leaveEntity!.Id,
            Status = LeaveStatus.Accepted
        };

        var result = await updateHandler.Handle(updateCommand, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();

        var updatedLeave = await context.Leaves.FindAsync(leaveEntity.Id);
        updatedLeave!.Status.Should().Be(LeaveStatus.Accepted);
    }
}