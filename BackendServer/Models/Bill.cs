namespace NWSDB5013.Models;

public class Bill
{
    public int BillId { get; set; }
    public DateTime IssuedDate { get; set; } = DateTime.Today;
    public DateTime DueDate { get; set; }
    public float TotalPayable { get; set; }
    public int UnitsUsed { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ConnId { get; set; }
    public int StaffId { get; set; }
    // reference navigation property so efcore maps fks properrly
    public WaterConnection WaterConnection { get; set; } = null!;
    public Staff Staff { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}