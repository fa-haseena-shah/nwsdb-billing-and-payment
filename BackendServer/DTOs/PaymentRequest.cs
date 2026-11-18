using System.ComponentModel.DataAnnotations;

namespace NWSDB5013.DTOs;
public class PaymentRequest
{
    [Required]
    public int BillId { get; set; }

    [Required]
    public float Amount { get; set; }

    [Required]
    public string IdempotencyKey { get; set; } = string.Empty;
    
    [Required]
    public string Role { get; set; } = string.Empty;

    [Required]
    public int CallerId { get; set; }
}