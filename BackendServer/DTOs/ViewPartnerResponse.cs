namespace NWSDB5013.DTOs;

public class ViewPartnerResponse
{
    public int PartnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegDate { get; set; }
    public string Status { get; set; } = string.Empty;
}