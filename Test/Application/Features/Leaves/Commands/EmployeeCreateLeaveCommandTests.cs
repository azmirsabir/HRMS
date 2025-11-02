using Application.Features.Leave.Command.AddLeave;
using Application.Interfaces;
using Core.Domain.Leave;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Test.Database.Seeds;

namespace Test.Application.Features.Leaves.Commands;

public class EmployeeCreateLeaveCommandTests
{
    [Fact]
    public async Task Handle_Should_Create_Leave_For_Employee()
    {
        var (context, aliUser, aliEmployee, managerUser, managerEmployee, auditorEmployee, auditorUser) = await DataSeed.SeedTestDataAsync();
            
        // Ali Request the leave
        var currentUserMock = new Mock<ICurrentUser>();
        currentUserMock.Setup(c => c.Id).Returns(aliUser.Id);
            
        // Handler
        var handler = new AddLeaveCommandHandler(context, currentUserMock.Object);

        var command = new AddLeaveCommand
        {
            LeaveType = LeaveType.Annual,
            Reason = "Vacation",
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 1, 5)
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var leaveInDb = await context.Leaves.FirstOrDefaultAsync();
        leaveInDb.Should().NotBeNull();
        leaveInDb.EmployeeId.Should().Be(1);
        leaveInDb.Status.Should().Be(LeaveStatus.Pending);
    }
}