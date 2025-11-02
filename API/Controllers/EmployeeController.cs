using Application.Common.Constants;
using Application.Features.Employee.Command.AddEmployee;
using Application.Features.Employee.Command.UpdateEmployee;
using Application.Features.Employee.Dto;
using Application.Features.Employee.Dto.Request;
using Application.Features.Employee.Query;
using Application.Features.Employee.Query.GetAllEmployee;
using Application.Features.Employee.Query.GetEmployeeById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/employee")]
[ApiController]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly ISender _sender;
    
    public EmployeeController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet]
    [Authorize(Roles = RoleConstants.Auditor+ ", " + RoleConstants.Manager+ ", " + RoleConstants.Employee)]
    public async Task<ActionResult> Get([FromQuery] GetEmployeeRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllEmployeeQuery()
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            DepartmentId = request.DepartmentId,
            Gender = request.Gender,
            FullName = request.FullName
        }, cancellationToken);
        return result.IsSuccess ? Ok(result.Data) : result.Error switch
        {
            _ => BadRequest(result.Errors)
        };
    }
    
    [HttpPost]
    [Authorize(Roles = RoleConstants.Auditor)]
    public async Task<IActionResult> CreateEmployee([FromBody] AddEmployeeRequest request)
    {
        var result = await _sender.Send(new AddEmployeeCommand
        {
            FullName = request.FullName,
            BaseSalary = request.BaseSalary,
            Gender = request.Gender,
            ServiceInYears = request.ServiceInYears,
            Degree = request.Degree
        });

        return result.IsSuccess
            ? Ok(result.Data)
            : result.Error switch
            {
                _ => BadRequest(result.Errors)
            };
    }
    
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Auditor")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var query = new GetEmployeeByIdQuery(id);

        var result = await _sender.Send(query);

        return result.IsSuccess
            ? Ok(result.Data)
            : result.Error switch
            {
                GetEmployeeByIdPossibleErrors.EmployeeNotFound => NotFound(new { Message = "Employee not found." }),
                _ => BadRequest(result.Errors)
            };
    }
    
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Auditor")]
    public async Task<IActionResult> UpdateEmployee(int id,[FromBody] UpdateEmployeeRequest request)
    {
        var command = new UpdateEmployeeCommand
        {
            Id = id,
            Degree = request.Degree,
            ServiceInYears = request.ServiceInYears,
            BaseSalary = request.BaseSalary
        };

        var result = await _sender.Send(command);

        return result.IsSuccess
            ? Ok(result.Data)
            : result.Error switch
            {
                UpdateEmployeePossibleErrors.EmployeeNotFound => NotFound(new { Message = "Employee not found." }),
                _ => BadRequest(result.Errors)
            };
    }
}