namespace NWSDB5013.Models;

public class Staff
{
    public int StaffId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public ICollection<Bill> BillsGenerated { get; set; } = new List<Bill>();
}