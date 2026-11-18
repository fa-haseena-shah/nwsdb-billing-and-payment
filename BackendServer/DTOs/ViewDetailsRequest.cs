using System.ComponentModel.DataAnnotations;

namespace NWSDB5013.DTOs;

public class ViewDetailsRequest
{
    [Required]
    public string AccountNo { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;

    [Required]
    public int CallerId { get; set; }
     public string? Nic { get; set; } // this will be used only if address is checked by user
}