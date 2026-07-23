namespace WebAppApi.Dto;

public class EmployeeCreateDto
{
    public string? Name { get; set; }
    public DateOnly? DOB { get; set; }
    public string? Position { get; set; }
    public string? Department { get; set; }
    public string? EmailAddress { get; set; }
}