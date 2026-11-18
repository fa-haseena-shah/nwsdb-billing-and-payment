namespace NWSDB5013.Services;

public class PartnerDeactivatedException : Exception
{
    public PartnerDeactivatedException() : base("This partner account has been deactivated.") { }
}