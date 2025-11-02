using Application.Contracts;
using Application.Features.Leave.Dto.Response;
using MediatR;

namespace Application.Features.Leave.Query.GetLeaveById;

public enum GetLeaveByIdPossibleErrors
{
    LeaveNotFound
}

public class GetLeaveByIdQuery : IRequest<Result<LeaveResponse, GetLeaveByIdPossibleErrors>>
{
    public int LeaveId { get; set; }

    public GetLeaveByIdQuery(int leaveId)
    {
        LeaveId = leaveId;
    }
}