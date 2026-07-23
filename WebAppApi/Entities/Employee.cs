using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppApi.Entities;

public class Employee
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public DateOnly? DOB { get; set; }

    [MaxLength(100)]
    public string? Position { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string? EmailAddress { get; set; }

    [ForeignKey("DepartmentId")]
    public int? DepartmentId { get; set; }
    public virtual Department? DepartmentEntity { get; set; }
}