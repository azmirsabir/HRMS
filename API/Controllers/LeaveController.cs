using Application.Common.Constants;
using Application.Features.Leave.Command.AddLeave;
using Application.Features.Leave.Command.UpdateLeave;
using Application.Features.Leave.Dto.Request;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GetAllLeaveQuery = Application.Features.Leave.Query.GetAllLeave.GetAllLeaveQuery;

namespace API.Controllers;

[Route("api/leaves")]
[ApiController]
[Authorize]
public class LeaveController : ControllerBase
{
    private readonly ISender _sender;
    
    public LeaveController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet]
    [Authorize(Roles = RoleConstants.Auditor+ ", " + RoleConstants.Manager+ ", " + RoleConstants.Employee)]
    public async Task<ActionResult> Get([FromQuery] GetLeaveRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllLeaveQuery()
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            EmployeeId = request.EmployeeId,
            Status = request.Status,
            Type = request.Type,
            FullName = request.FullName
        }, cancellationToken);
        return result.IsSuccess ? Ok(result.Data) : result.Error switch
        {
            _ => BadRequest(result.Errors)
        };
    }
    
    [HttpPost]
    [Authorize(Roles = RoleConstants.Employee)]
    public async Task<IActionResult> CreateLeave([FromBody] AddLeaveRequest request)
    {
        var result = await _sender.Send(new AddLeaveCommand
        {
            LeaveType = request.LeaveType,
            Reason = request.Reason,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        });

        return result.IsSuccess
            ? Ok(result.Data)
            : result.Error switch
            {
                _ => BadRequest(result.Errors)
            };
    }
    
    [HttpPut("{id:int}/approve")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> UpdateLeave(int id,[FromBody] UpdateLeaveRequest request)
    {
        var command = new UpdateLeaveCommand()
        {
            Id = id,
            Status= request.status,
            RejectionReason = request.RejectionReason
        };
    
        var result = await _sender.Send(command);
    
        return result.IsSuccess
            ? Ok(result.Data)
            : result.Error switch
            {
                UpdateLeavePossibleErrors.LeaveNotFound => NotFound(new { Message = "Leave not found." }),
                UpdateLeavePossibleErrors.LeaveAlreadyApproved => NotFound(new { Message = "Leave already approved." }),
                UpdateLeavePossibleErrors.InvalidEmployeeDepartment => NotFound(new { Message = "The Manager can't Approve, because Manager and Requester department are different ." }),
                _ => BadRequest(result.Errors)
            };
    }
}