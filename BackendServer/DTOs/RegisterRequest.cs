using System.ComponentModel.DataAnnotations;

namespace NWSDB5013.DTOs;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage ="Password must be atleast 8 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength (100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength (12, ErrorMessage ="NIC number cannot be longer than 12 characters.")]
    public string NIC { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNo { get; set; } = string.Empty;
}