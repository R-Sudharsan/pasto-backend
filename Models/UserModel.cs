
using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    public string? Salt { get; set; }

    // Additional user properties and methods can be added here
}
