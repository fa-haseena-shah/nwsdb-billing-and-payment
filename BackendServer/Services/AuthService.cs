using NWSDB5013.Data;
using NWSDB5013.DTOs;
using NWSDB5013.Models;
using Microsoft.EntityFrameworkCore;

namespace NWSDB5013.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    public AuthService(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // used for customer, staff and partner login
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        // clean email
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // first, check if its customer
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == normalizedEmail);
        if (customer is not null)
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash))
                return null;

            return new AuthResponse
            {
                UserId = customer.CustomerId,
                FullName = customer.FullName,
                Email = customer.Email,
                Role = "Customer"
            };
        }

        // otherwise, check if its staff
        var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == normalizedEmail);
        if (staff is not null)
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, staff.PasswordHash))
                return null;

            return new AuthResponse
            {
                UserId = staff.StaffId,
                FullName = staff.FullName,
                Email = staff.Email,
                Role = "Staff"
            };
        }

        // finally check if its partner
        var partner = await _context.Partners.FirstOrDefaultAsync(p => p.Email == normalizedEmail);
        if (partner is not null)
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, partner.PasswordHash))
            return null;

            if (partner.Status != "Activated")
                throw new PartnerDeactivatedException();

            return new AuthResponse
            {
                UserId = partner.PartnerId,
                FullName = partner.PartnerName,
                Email = partner.Email,
                Role = "Partner"
            };
        }
        // none of them
        return null;
    }

    
    // customer ONLY registration
    public async Task<AuthResponse?> RegisterCustomerAsync(RegisterRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // cannot use an already used email by staff or partner even
        bool emailTaken = await _context.Customers.AnyAsync(c => c.Email == normalizedEmail)
        || await _context.Staff.AnyAsync(s => s.Email == normalizedEmail)
        || await _context.Partners.AnyAsync(p => p.Email == normalizedEmail);

        if (emailTaken)
            return null;

        // new customer instance to save to db
        var customer = new Customer
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            NIC = request.NIC.Trim(),
            PhoneNo = request.PhoneNo.Trim()
        };

        // save customer to db
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            UserId = customer.CustomerId,
            FullName = customer.FullName,
            Email = customer.Email,
            Role = "Customer"
        };
    }

    public async Task<ViewPartnerResponse?> CreatePartnerAsync(CreatePartnerRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        bool emailTaken = await _context.Customers.AnyAsync(c => c.Email == normalizedEmail)
            || await _context.Staff.AnyAsync(s => s.Email == normalizedEmail)
            || await _context.Partners.AnyAsync(p => p.Email == normalizedEmail);

        if (emailTaken) return null;

        var generatedPassword = GenerateRandomPassword();

        var partner = new Partner
        {
            PartnerName = request.Name.Trim(),
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(generatedPassword),
            CreatedAt = DateTime.UtcNow,
            Status = "Activated"
        };

        _context.Partners.Add(partner);
        await _context.SaveChangesAsync();

        // send credentials by email
        var subject = "Your NWSDB Partner Account Credentials";
        var body = $"""
            Dear {partner.PartnerName},

            We are pleased to inform that an NWSDB Partner account has been created for you as per your request!

            Login Email: {partner.Email}
            Temporary Password: {generatedPassword}

            Please log in and keep these credentials secure.

            This is an automated message from the Sri Lankan National Water Supply and Drainage Board.
            """;

        await _emailService.SendEmailAsync(partner.Email, subject, body);

        return new ViewPartnerResponse
        {
            PartnerId = partner.PartnerId,
            Name = partner.PartnerName,
            Email = partner.Email,
            RegDate = partner.CreatedAt,
            Status = partner.Status
        };
    }

    private string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%";
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(12);
        var result = new char[12];

        for (int i = 0; i < 12; i++)
            result[i] = chars[bytes[i] % chars.Length];

        return new string(result);
    }
}