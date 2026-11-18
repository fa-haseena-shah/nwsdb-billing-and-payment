using System.ComponentModel.DataAnnotations;

namespace NWSDB5013.DTOs;

public class CreateBillRequest
{
    [Required]
    public string AccountNo { get; set; } = string.Empty;

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public float TotalPayable { get; set; }

    [Required]
    public int StaffId { get; set; }
}