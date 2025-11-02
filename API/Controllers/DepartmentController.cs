using Application.Common.Constants;
using Application.Features.Department.Command.AddDepartment;
using Application.Features.Department.Dto;
using Application.Features.Department.Dto.Request;
using Application.Features.Department.Query;
using Application.Features.Department.Query.GetAllDepartment;
using Application.Features.Leave.Command.AddLeave;
using Application.Features.Leave.Command.UpdateLeave;
using Application.Features.Leave.Dto;
using Application.Features.Leave.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/departments")]
[ApiController]
[Authorize]
public class DepartmentController : ControllerBase
{
    private readonly ISender _sender;
    
    public DepartmentController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet]
    [Authorize(Roles = RoleConstants.Auditor+ ", " + RoleConstants.Manager+ ", " + RoleConstants.Employee)]
    public async Task<ActionResult> Get([FromQuery] GetDepartmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllDepartmentQuery()
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            DepartmentId = request.Id,
            Name = request.Name
        }, cancellationToken);
        return result.IsSuccess ? Ok(result.Data) : result.Error switch
        {
            _ => BadRequest(result.Errors)
        };
    }
    
    [HttpPost]
    [Authorize(Roles = RoleConstants.Auditor)]
    public async Task<IActionResult> CreateDepartment([FromBody] AddDepartmentRequest request)
    {
        var result = await _sender.Send(new AddDepartmentCommand
        {
            Name = request.Name,
        });

        return result.IsSuccess
            ? Ok(result.Data)
            : result.Error switch
            {
                _ => BadRequest(result.Errors)
            };
    }
}