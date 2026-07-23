using FluentValidation;
using WebAppApi.Dto;

namespace WebAppApi.Validators;

public class EmployeeUpdateDtoValidator : AbstractValidator<EmployeeUpdateDto>
{
    public EmployeeUpdateDtoValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters.");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Position)
            .MaximumLength(100).WithMessage("Position must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Position));

        RuleFor(x => x.Department)
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Department));

        RuleFor(x => x.DOB)
            .LessThan(DateOnly.FromDateTime(DateTime.Today.AddYears(-16)))
            .WithMessage("Employee must be at least 16 years old.")
            .When(x => x.DOB.HasValue);
    }
}
