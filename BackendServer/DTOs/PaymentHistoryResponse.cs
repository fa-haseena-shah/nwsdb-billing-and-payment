using System.ComponentModel.DataAnnotations;

namespace NWSDB5013.DTOs;

public class PaymentHistoryResponse
{
    public int PaymentId { get; set; }
    public DateTime Date { get; set; }
    public float Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int BillId { get; set; }
}