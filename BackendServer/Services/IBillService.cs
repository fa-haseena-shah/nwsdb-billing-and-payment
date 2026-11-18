using NWSDB5013.DTOs;

namespace NWSDB5013.Services;

public interface IBillService
{
    Task<ViewBillResponse?> GenerateBillAsync(CreateBillRequest request);
}