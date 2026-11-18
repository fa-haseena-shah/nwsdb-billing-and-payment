namespace NWSDB5013.Models;

public class MeterReading
{
    public int ReadingId { get; set; }
    public DateTime ReadingDate { get; set; } = DateTime.Today;
    public int CurrentValue { get; set; }
    public int ConnId { get; set; }
    public WaterConnection WaterConnection { get; set; } = null!;
}