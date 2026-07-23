using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppApi.Dto;
using WebAppApi.GenericResponse;
using WebAppApi.IService;

namespace WebAppApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResponseResult<IEnumerable<EmployeeDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<IEnumerable<EmployeeDto>>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllEmployees(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _employeeService.GetAllEmployeesAsync(pageNumber, pageSize);
        var message = result.Message ?? "Employees retrieved successfully.";

        return Ok(ResponseResult<IEnumerable<EmployeeDto>>.Success(
            result.Data,
            message,
            new
            {
                result.PageNumber,
                result.PageSize,
                result.TotalCount,
                result.TotalPages,
                result.HasNextPage,
                result.HasPreviousPage
            }));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ResponseResult<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<EmployeeDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseResult<EmployeeDto>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetEmployeeById(Guid id)
    {
        var result = await _employeeService.GetEmployeeById(id);
        var message = result.Message ?? "Request could not be completed.";

        if (!result.IsSuccess)
        {
            return NotFound(ResponseResult<EmployeeDto>.Failure(null, message));
        }

        return Ok(ResponseResult<EmployeeDto>.Success(result.Data, message));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseResult<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDto employeeCreateDto)
    {
        var result = await _employeeService.CreateEmployee(employeeCreateDto);
        var message = result.Message ?? "Request could not be completed.";

        if (!result.IsSuccess)
        {
            return BadRequest(ResponseResult<string>.Failure(null, message));
        }

        return Ok(ResponseResult<string>.Success(null, message));
    }

    [HttpPut]
    [ProducesResponseType(typeof(ResponseResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseResult<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeUpdateDto employeeUpdateDto)
    {
        var result = await _employeeService.UpdateEmployee(employeeUpdateDto);
        var message = result.Message ?? "Request could not be completed.";

        if (!result.IsSuccess)
        {
            return BadRequest(ResponseResult<string>.Failure(null, message));
        }

        return Ok(ResponseResult<string>.Success(null, message));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ResponseResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseResult<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteEmployee(Guid id)
    {
        var result = await _employeeService.DeleteEmployee(id);
        var message = result.Message ?? "Request could not be completed.";

        if (!result.IsSuccess)
        {
            return NotFound(ResponseResult<string>.Failure(null, message));
        }

        return Ok(ResponseResult<string>.Success(null, message));
    }
}
