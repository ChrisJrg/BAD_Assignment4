using System.ComponentModel.DataAnnotations;

namespace AarhusSpaceProgramAPI.Models;

public class LoginDto
{
    [Required]
    [MaxLength(255)]
    public string UserName { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } =  string.Empty;
}