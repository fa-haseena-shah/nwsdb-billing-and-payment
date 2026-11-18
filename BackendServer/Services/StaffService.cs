using Microsoft.EntityFrameworkCore;
using NWSDB5013.Data;
using NWSDB5013.DTOs;

namespace NWSDB5013.Services;

public class StaffService : IStaffService
{
    private readonly AppDbContext _context;

    public StaffService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PageResult<ListWaterConn>> GetAllWaterConnectionsAsync(int pageNumber, int pageSize)
    {
        int totalCount = await _context.WaterConnections.CountAsync();

        var items = await _context.WaterConnections
            .AsNoTracking()
            .Include(w => w.Customer)
            .OrderBy(w => w.ConnId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(w => new ListWaterConn
            {
                ConnId = w.ConnId,
                AccountNo = w.AccNo,
                Status = w.Status,
                CustomerName = w.Customer.FullName
            })
            .ToListAsync();

        return new PageResult<ListWaterConn>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PageResult<ListCustomer>> GetAllCustomersAsync(int pageNumber, int pageSize)
    {
        int totalCount = await _context.Customers.CountAsync();

        var items = await _context.Customers
            .AsNoTracking()
            .OrderBy(c => c.CustomerId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ListCustomer
            {
                CustomerId = c.CustomerId,
                FullName = c.FullName,
                PhoneNo = c.PhoneNo
            })
            .ToListAsync();

        return new PageResult<ListCustomer>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PageResult<ListPartner>> GetAllPartnersAsync(int pageNumber, int pageSize)
    {
        int totalCount = await _context.Partners.CountAsync();

        var items = await _context.Partners
            .AsNoTracking()
            .OrderBy(p => p.PartnerId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ListPartner
            {
                PartnerId = p.PartnerId,
                Name = p.PartnerName,
                Email = p.Email,
                Status = p.Status
            })
            .ToListAsync();

        return new PageResult<ListPartner>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
    public async Task<ViewPartnerResponse?> UpdatePartnerStatusAsync(UpdatePartnerRequest request)
    {
        if (request.Status != "Activated" && request.Status != "Deactivated")
            return null;

        var partner = await _context.Partners
            .FirstOrDefaultAsync(p => p.PartnerId == request.PartnerId);

        if (partner is null) return null;

        partner.Status = request.Status;
        await _context.SaveChangesAsync();

        return new ViewPartnerResponse
        {
            PartnerId = partner.PartnerId,
            Name = partner.PartnerName,
            Email = partner.Email,
            RegDate = partner.CreatedAt,
            Status = partner.Status
        };
    }
}