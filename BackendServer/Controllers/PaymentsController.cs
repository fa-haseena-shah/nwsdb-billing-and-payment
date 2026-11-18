using Microsoft.AspNetCore.Mvc;
using NWSDB5013.DTOs;
using NWSDB5013.Services;

namespace NWSDB5013.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService, IReceiptService receiptService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("pay")]
    public async Task<IActionResult> PayBill([FromBody] PaymentRequest request)
    {
        var result = await _paymentService.PayBillAsync(request);

        if (result is null)
            return NotFound(new { message = "Bill not found, already paid, access denied, or amount mismatch." });

        return Ok(result);
    }
}