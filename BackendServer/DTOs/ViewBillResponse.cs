using System.ComponentModel.DataAnnotations;

namespace NWSDB5013.DTOs;

public class ViewBillResponse
{
    public int BillId { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime DueDate { get; set; }
    public float TotalPayable { get; set; }
    public int UnitsUsed { get; set; }
    public string Status { get; set; } = string.Empty;
}