namespace NWSDB5013.Models;

public class Receipt
{
    public int ReceiptId { get; set; }
    public int PaymentId { get; set; }
    public DateTime GenerationTime { get; set; } = DateTime.UtcNow;
    public Payment Payment { get; set; } = null!;
}