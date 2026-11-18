namespace NWSDB5013.Models;

public class WaterConnection
{
    public int ConnId { get; set; }
    public string AccNo { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public ICollection<Bill> Bills { get; set; }
    = new List<Bill>();

    public ICollection<MeterReading> MeterReadings { get; set; }
    = new List<MeterReading>();
}