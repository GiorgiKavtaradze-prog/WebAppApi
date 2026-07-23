using Microsoft.AspNetCore.Mvc;
using WebAppApi.Dto;
using WebAppApi.GenericResponse;
using WebAppApi.IService;

namespace WebAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var result = await _employeeService.GetAllEmployeeAsync();
                return Ok(ResponseResult<List<EmployeeDto>>.Success(result.Item2, result.Item2.Count > 0 ? "Employees retrieved successfully" : "No employees found"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
            try
            {
                var result = await _employeeService.GetEmployeeById(id);
                
                if (result.Item1 == 0)
                {
                    return NotFound(ResponseResult<EmployeeDto>.Failure(null, "Employee not found"));
                }
                
                return Ok(ResponseResult<EmployeeDto>.Success(result.Item2, "Employee retrieved successfully"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDto employeeDto)
        {
            try
            {
                var result = await _employeeService.CreateEmployee(employeeDto);
                
                if (result.Item1 == 0)
                {
                    return BadRequest(ResponseResult<string>.Failure(null, result.Item2));
                }
                
                return Ok(ResponseResult<string>.Success(null, result.Item2));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeDto employeeDto)
        {
            try
            {
                var result = await _employeeService.UpdateEmployee(employeeDto);
                
                if (result.Item1 == 0)
                {
                    return BadRequest(ResponseResult<string>.Failure(null, result.Item2));
                }
                
                return Ok(ResponseResult<string>.Success(null, result.Item2));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                var result = await _employeeService.DeleteEmployee(id);
                
                if (result.Item1 == 0)
                {
                    return NotFound(ResponseResult<string>.Failure(null, result.Item2));
                }
                
                return Ok(ResponseResult<string>.Success(null, result.Item2));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}