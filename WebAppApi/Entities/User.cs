using System.ComponentModel.DataAnnotations;

namespace WebAppApi.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }
}