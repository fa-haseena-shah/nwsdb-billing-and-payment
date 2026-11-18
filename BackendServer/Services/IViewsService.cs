using CSE5013_NWSDB.DTOs;
using NWSDB5013.DTOs;
using static NWSDB5013.Services.ViewsService;
namespace NWSDB5013.Services;

public interface IViewsService
{
    Task<(ViewBillResponse? bill, AccessDenialReason reason)> GetViewBillAsync(string accountNo, string role, int callerId);    Task<ViewUsageResponse?> GetViewUsageAsync(string accountNo, string role, int callerId);
    Task<List<PaymentHistoryResponse>?> GetPaymentHistoriesAsync(string accountNo, string role, int callerId);
    Task<List<PaymentHistoryResponse>?> GetPartnerPaymentsAsync(int partnerId);
    Task<bool> AccountExistsAsync(string accountNo);
    Task<ViewAccResponse?> GetAccountDetailsAsync(string accountNo, string role, int callerId, string? nic);
    Task<StaffViewAccResponse?> GetStaffAccountDetailsAsync(string accountNo);
    Task<CreateBillPreviewResponse?> GetBillPrepAsync(string accountNo);
}