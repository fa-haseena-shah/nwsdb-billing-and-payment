using NWSDB5013.DTOs;

namespace NWSDB5013.Services;

public interface IStaffService
{
    Task<PageResult<ListWaterConn>> GetAllWaterConnectionsAsync(int pageNumber, int pageSize);
    Task<PageResult<ListCustomer>> GetAllCustomersAsync(int pageNumber, int pageSize);
    Task<PageResult<ListPartner>> GetAllPartnersAsync(int pageNumber, int pageSize);
    Task<ViewPartnerResponse?> UpdatePartnerStatusAsync(UpdatePartnerRequest request);
}