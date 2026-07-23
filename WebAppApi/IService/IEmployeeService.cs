using WebAppApi.Common;
using WebAppApi.Dto;

namespace WebAppApi.IService;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeDto>> GetAllEmployeesAsync(int pageNumber = 1, int pageSize = 10);
    Task<Result<EmployeeDto>> GetEmployeeById(Guid id);
    Task<Result> CreateEmployee(EmployeeCreateDto employee);
    Task<Result> UpdateEmployee(EmployeeUpdateDto employee);
    Task<Result> DeleteEmployee(Guid id);
}