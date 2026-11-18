namespace NWSDB5013.DTOs;

public class PaymentResponse
{
    public int PaymentId { get; set; }
    public DateTime Date { get; set; }
    public float Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PgRef { get; set; } = string.Empty;
    public int ReceiptId { get; set; }
}