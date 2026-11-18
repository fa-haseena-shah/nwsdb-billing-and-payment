namespace NWSDB5013.Models;

public class Payment
{
    public int PaymentId { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.Now;
    public float TotalPaid { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PGRef { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;

    // using int? cuz it CAN be null
    public int? CustomerId { get; set; }
    public int? PartnerId { get; set; }
    public int BillId { get; set; }
    
    // navigation properties
    public Customer? Customer { get; set; }
    public Partner? Partner { get; set; }
    public Bill Bill { get; set; } = null!;
    public Receipt? Receipt { get; set; }
}