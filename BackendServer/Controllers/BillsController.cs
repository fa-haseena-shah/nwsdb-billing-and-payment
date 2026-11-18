using Microsoft.AspNetCore.Mvc;
using NWSDB5013.DTOs;
using NWSDB5013.Services;

namespace NWSDB5013.Controllers;
[ApiController]
[Route("api/bills")]
public class BillsController : ControllerBase
{
    private readonly IBillService _billService;

    public BillsController(IBillService billService)
    {
        _billService = billService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateBill([FromBody] CreateBillRequest request)
    {
        try
        {
            var result = await _billService.GenerateBillAsync(request);

            if (result is null)
                return NotFound(new { message = "Account or staff member not found." });

            return Ok(result);
        } catch (InvalidOperationException e)
        {
            return Conflict(new { message = e.Message });
        }
    }
}