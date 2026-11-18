namespace NWSDB5013.DTOs;

public class CreateBillPreviewResponse
{
    public string AccountNo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<ViewMeterReading> Readings { get; set; } = new();
}