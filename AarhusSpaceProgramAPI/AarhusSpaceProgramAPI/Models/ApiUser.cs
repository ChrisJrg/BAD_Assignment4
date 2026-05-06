using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AarhusSpaceProgramAPI.Models;

public class ApiUser : IdentityUser
{
    [MaxLength(100)]
    public string? FullName { get; set; }
}