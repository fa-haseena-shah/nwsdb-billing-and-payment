using Microsoft.AspNetCore.Mvc;
using NWSDB5013.DTOs;
using NWSDB5013.Services;

namespace NWSDB5013.Controllers;

[ApiController]
[Route("api/staff")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [HttpGet("water-connections")]
    public async Task<IActionResult> GetAllWaterConnections([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _staffService.GetAllWaterConnectionsAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetAllCustomers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _staffService.GetAllCustomersAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("partners")]
    public async Task<IActionResult> GetAllPartners([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _staffService.GetAllPartnersAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpPost("partners/update-status")]
    public async Task<IActionResult> UpdatePartnerStatus([FromBody] UpdatePartnerRequest request)
    {
        var result = await _staffService.UpdatePartnerStatusAsync(request);

        if (result is null)
            return NotFound(new { message = "Partner not found or invalid status value." });

        return Ok(result);
    }
}