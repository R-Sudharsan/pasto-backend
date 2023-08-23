
using System.ComponentModel.DataAnnotations;

public class Paste
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string PastedContent { get; set; }

    [Required]
    public bool IsPrivate { get; set; }

    public string CollaboratorEmailList { get; set; }

    [Required]
    public string OwnerEmail { get; set; }

    // Additional properties and methods can be added here
}
