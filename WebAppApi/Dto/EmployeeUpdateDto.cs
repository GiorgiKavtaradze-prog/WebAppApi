namespace WebAppApi.Dto;

public class EmployeeUpdateDto
{
    public string? Name { get; set; }
    public DateOnly? DOB { get; set; }
    public string? Position { get; set; }
    public string? Department { get; set; }
    public string? EmailAddress { get; set; }
}