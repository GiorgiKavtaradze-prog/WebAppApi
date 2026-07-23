using System.ComponentModel.DataAnnotations;

namespace WebAppApi.Entities;

public class Department
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public virtual ICollection<Employee>? Employees { get; set; }
}