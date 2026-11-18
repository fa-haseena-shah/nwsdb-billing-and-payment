using System.ComponentModel.DataAnnotations;

namespace NWSDB5013.DTOs;

public class UpdatePartnerRequest
{
    [Required]
    public int PartnerId { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;
}