namespace NWSDB5013.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string NIC { get; set; } = string.Empty;
    public string PhoneNo { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // navigation property to water connections and payments
    public ICollection<WaterConnection> WaterConnections { get; set; } 
    = new List<WaterConnection>();
    
    public ICollection<Payment> Payments { get; set; }
     = new List<Payment>();
}