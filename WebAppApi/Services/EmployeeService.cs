using Microsoft.EntityFrameworkCore;
using WebAppApi.Common;
using WebAppApi.Data;
using WebAppApi.Dto;
using WebAppApi.IService;

namespace WebAppApi.Services;

public class EmployeeService : IEmployeeService
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;
    private const string InvalidRequestMessage = "Please provide all required details.";

    private readonly AppDbContext _context;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(AppDbContext context, ILogger<EmployeeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<EmployeeDto>> GetAllEmployeesAsync(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            pageNumber = pageNumber <= 0 ? DefaultPageNumber : pageNumber;
            pageSize = pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);

            var totalCount = await _context.Employees.CountAsync();

            var employees = await _context.Employees
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new EmployeeDto
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    LastModifiedDate = x.LastModifiedDate,
                    Department = x.Department,
                    DOB = x.DOB,
                    Name = x.Name,
                    EmailAddress = x.EmailAddress,
                    Position = x.Position
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} employees (page {PageNumber})", employees.Count, pageNumber);
            return PagedResult<EmployeeDto>.Success(employees, pageNumber, pageSize, totalCount, "Employees retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employees");
            throw;
        }
    }

    public async Task<Result<EmployeeDto>> GetEmployeeById(Guid id)
    {
        try
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new EmployeeDto
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    LastModifiedDate = x.LastModifiedDate,
                    Department = x.Department,
                    DOB = x.DOB,
                    Name = x.Name,
                    EmailAddress = x.EmailAddress,
                    Position = x.Position
                })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                _logger.LogWarning("Employee not found with ID: {Id}", id);
                return Result<EmployeeDto>.Failure("Employee not found");
            }

            _logger.LogInformation("Retrieved employee with ID: {Id}", id);
            return Result<EmployeeDto>.Success(employee, "Employee retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employee with ID: {Id}", id);
            throw;
        }
    }

    public async Task<Result> CreateEmployee(EmployeeCreateDto employee)
    {
        try
        {
            if (employee is null
                || string.IsNullOrWhiteSpace(employee.Name)
                || string.IsNullOrWhiteSpace(employee.EmailAddress))
            {
                return Result.Failure(InvalidRequestMessage);
            }

            var normalizedEmail = employee.EmailAddress.Trim().ToLowerInvariant();
            var existing = await _context.Employees
                .AsNoTracking()
                .AnyAsync(x => x.EmailAddress == normalizedEmail);

            if (existing)
            {
                return Result.Failure("Employee already exists with the same email address");
            }

            var entity = new Entities.Employee
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = null,
                Department = employee.Department?.Trim(),
                DOB = employee.DOB,
                Name = employee.Name.Trim(),
                EmailAddress = normalizedEmail,
                Position = employee.Position?.Trim()
            };

            await _context.Employees.AddAsync(entity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created employee with email: {Email}", normalizedEmail);
            return Result.Success("Employee created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee with email: {Email}", employee?.EmailAddress);
            throw;
        }
    }

    public async Task<Result> UpdateEmployee(EmployeeUpdateDto employee)
    {
        try
        {
            if (employee is null || string.IsNullOrWhiteSpace(employee.EmailAddress))
            {
                return Result.Failure(InvalidRequestMessage);
            }

            var normalizedEmail = employee.EmailAddress.Trim().ToLowerInvariant();
            var existing = await _context.Employees
                .FirstOrDefaultAsync(x => x.EmailAddress == normalizedEmail);

            if (existing == null)
            {
                return Result.Failure("Employee not found with this email address");
            }

            existing.Position = string.IsNullOrWhiteSpace(employee.Position) ? existing.Position : employee.Position.Trim();
            existing.DOB = employee.DOB ?? existing.DOB;
            existing.Name = string.IsNullOrWhiteSpace(employee.Name) ? existing.Name : employee.Name.Trim();
            existing.EmailAddress = normalizedEmail;
            existing.Department = string.IsNullOrWhiteSpace(employee.Department) ? existing.Department : employee.Department.Trim();
            existing.LastModifiedDate = DateTime.UtcNow;

            _context.Employees.Update(existing);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated employee with email: {Email}", normalizedEmail);
            return Result.Success("Employee updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee with email: {Email}", employee?.EmailAddress);
            throw;
        }
    }

    public async Task<Result> DeleteEmployee(Guid id)
    {
        try
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
            {
                return Result.Failure("Employee not found with this ID");
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted employee with ID: {Id}", id);
            return Result.Success("Employee deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting employee with ID: {Id}", id);
            throw;
        }
    }
}
