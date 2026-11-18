using Microsoft.AspNetCore.Mvc;
using NWSDB5013.DTOs;
using NWSDB5013.Services;

namespace NWSDB5013.Controllers;
[ApiController]
[Route("api/views")]
public class ViewController : ControllerBase
{
    private readonly IViewsService _viewService;

    public ViewController(IViewsService viewService)
    {
        _viewService = viewService;
    }


    [HttpGet("account-exists/{accountNo}")]
    public async Task<IActionResult> AccountExists(string accountNo)
    {
        var exists = await _viewService.AccountExistsAsync(accountNo);
        return Ok(new { exists });
    }

    // bill view
    [HttpPost("bill")]
    public async Task<IActionResult> ViewBill([FromBody] ViewDetailsRequest request)
    {
        var (bill, reason) = await _viewService.GetViewBillAsync(request.AccountNo, request.Role, request.CallerId);

        if (bill is null)
        {
            var message = reason switch
            {
                ViewsService.AccessDenialReason.AccountNotFound => "No connection found with that number.",
                ViewsService.AccessDenialReason.NotOwnedByCustomer => "This connection is not registered under your profile.",
                ViewsService.AccessDenialReason.AccountInactiveForPartner => "This connection is not currently active.",
                ViewsService.AccessDenialReason.NoBillAvailable => "No bill is currently available for this account.",
                _ => "Bill could not be retrieved."
            };
            return NotFound(new { message });
        }

        return Ok(bill);
    }

    // usage details view
    [HttpPost("usage")]
    public async Task<IActionResult> ViewUsage([FromBody] ViewDetailsRequest request)
    {
        var result = await _viewService.GetViewUsageAsync(request.AccountNo, request.Role, request.CallerId);

        if (result is null)
            return NotFound(new { message = "Account not found or access denied." });

        return Ok(result);
    }

    // payment history
    [HttpPost("payment-history")]
    public async Task<IActionResult> ViewPaymentHistory([FromBody] ViewDetailsRequest request)
    {
        var result = await _viewService.GetPaymentHistoriesAsync(request.AccountNo, request.Role, request.CallerId);

        if (result is null)
            return NotFound(new { message = "Account not found or access denied." });

        return Ok(result);
    }

    [HttpGet("partner-payments/{partnerId}")]
    public async Task<IActionResult> PartnerPayments(int partnerId)
    {
        var result = await _viewService.GetPartnerPaymentsAsync(partnerId);

        if (result is null)
            return NotFound(new { message = "Partner not found." });

        return Ok(result);
    }

    [HttpPost("account-details")]
    public async Task<IActionResult> ViewAccountDetails([FromBody] ViewDetailsRequest request)
    {
        try
        {
            var result = await _viewService.GetAccountDetailsAsync(
                request.AccountNo, request.Role, request.CallerId, request.Nic);

            if (result is null)
                return NotFound(new { message = "Account not found or access denied." });

            return Ok(result);
        }
        catch (NicMismatchException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }

    [HttpGet("staff-account-details/{accountNo}")]
    public async Task<IActionResult> StaffAccountDetails(string accountNo)
    {
        var result = await _viewService.GetStaffAccountDetailsAsync(accountNo);

        if (result is null)
            return NotFound(new { message = "Account not found." });

        return Ok(result);
    }

    [HttpGet("staff-bill-prep/{accountNo}")]
    public async Task<IActionResult> StaffBillPrep(string accountNo)
    {
        var result = await _viewService.GetBillPrepAsync(accountNo);

        if (result is null)
            return NotFound(new { message = "Account not found." });

        return Ok(result);
    }
}