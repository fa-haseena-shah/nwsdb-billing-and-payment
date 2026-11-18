namespace NWSDB5013.Services;
public class NicMismatchException : Exception
{
    public NicMismatchException() : base("The NIC entered does not match our records for this account.") { }
}