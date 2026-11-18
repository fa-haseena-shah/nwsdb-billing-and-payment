namespace CSE5013_NWSDB.DTOs;
using NWSDB5013.DTOs;

public class ViewUsageResponse
{
    public string AccountNo { get; set; } = string.Empty;

    // list of the meter readings so that usage can be viewed over time instead
    // of just random readings
    public List<ViewMeterReading> Readings { get; set; } = new();
}