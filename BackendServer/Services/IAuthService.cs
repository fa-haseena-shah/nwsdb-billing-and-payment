using NWSDB5013.DTOs;
namespace NWSDB5013.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RegisterCustomerAsync(RegisterRequest request);
    Task<ViewPartnerResponse?> CreatePartnerAsync(CreatePartnerRequest request);
}