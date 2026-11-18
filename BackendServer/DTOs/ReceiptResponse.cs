namespace NWSDB5013.DTOs;

public class ReceiptResponse
{
    public int ReceiptId { get; set; }
    public int PaymentId { get; set; }
    public DateTime PaymentDate { get; set; }
    public float Amount { get; set; }
    public int BillId { get; set; }
    public string AccountNo { get; set; } = string.Empty;
}