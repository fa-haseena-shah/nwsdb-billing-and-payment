using NWSDB5013.DTOs;
using NWSDB5013.Services;
using Microsoft.AspNetCore.Mvc;

namespace NWSDB5013.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterCustomerAsync(request);

        if (result is null)
            return Conflict(new { message = "This email is already registered." });

        return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);

            if (result is null)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(result);
        }
        catch (PartnerDeactivatedException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }

    [HttpPost("staff/create-partner")]
    public async Task<IActionResult> CreatePartner([FromBody] CreatePartnerRequest request)
    {
        var result = await _authService.CreatePartnerAsync(request);

        if (result is null)
            return Conflict(new { message = "This email is already registered." });

        return CreatedAtAction(nameof(CreatePartner), new { id = result.PartnerId }, result);
    }
}