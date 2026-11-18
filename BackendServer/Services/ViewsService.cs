using NWSDB5013.Data;
using NWSDB5013.DTOs;
using NWSDB5013.Models;
using Microsoft.EntityFrameworkCore;
using CSE5013_NWSDB.DTOs;

namespace NWSDB5013.Services;

public class ViewsService : IViewsService
{
    private readonly AppDbContext _context;
    public enum AccessDenialReason
    {
        None,
        AccountNotFound,
        NotOwnedByCustomer,
        AccountInactiveForPartner,
        NoBillAvailable
    }
    public ViewsService(AppDbContext context)
    {
        _context = context;
    }

    private async Task<WaterConnection?> GetAuthorizedConnectionAsync(string accountNo, string role, int callerId)
    {
        var connection = await _context.WaterConnections
            .Include(w => w.Customer)
            .FirstOrDefaultAsync(w => w.AccNo == accountNo);

        if (connection is null) return null;

        if (role == "Customer" && connection.CustomerId != callerId)
            return null;

        if (role == "Partner" && connection.Status != "Active")
            return null;

        return connection;
    }

    // customer, partner, staff may view bills
    public async Task<(ViewBillResponse? bill, AccessDenialReason reason)> GetViewBillAsync(string accountNo, string role, int callerId)
    {
        var connection = await _context.WaterConnections
            .Include(w => w.Customer)
            .FirstOrDefaultAsync(w => w.AccNo == accountNo);

        if (connection is null)
            return (null, AccessDenialReason.AccountNotFound);

        if (role == "Customer" && connection.CustomerId != callerId)
            return (null, AccessDenialReason.NotOwnedByCustomer);

        if (role == "Partner" && connection.Status != "Active")
            return (null, AccessDenialReason.AccountInactiveForPartner);

        var bill = await _context.Bills
            .Where(b => b.ConnId == connection.ConnId)
            .OrderByDescending(b => b.IssuedDate)
            .FirstOrDefaultAsync();

        if (bill is null)
            return (null, AccessDenialReason.NoBillAvailable);

        return (new ViewBillResponse
        {
            BillId = bill.BillId,
            IssuedDate = bill.IssuedDate,
            DueDate = bill.DueDate,
            TotalPayable = bill.TotalPayable,
            UnitsUsed = bill.UnitsUsed,
            Status = bill.Status
        }, AccessDenialReason.None);
    }

    public async Task<ViewUsageResponse?> GetViewUsageAsync(string accountNo, string role, int callerId)
    {
        // customer and staff only
        if (role == "Partner") return null;

        var connection = await GetAuthorizedConnectionAsync(accountNo, role, callerId);
        if (connection is null) return null;

        var readings = await GetReadingsAsync(connection.ConnId);

        return new ViewUsageResponse
        {
            AccountNo = accountNo,
            Readings = readings
        };
    }

    public async Task<List<PaymentHistoryResponse>?> GetPaymentHistoriesAsync(string accountNo, string role, int callerId)
    {
        // getting payment history by water account - customer or staff only
        if (role == "Partner") return null;

        var connection = await GetAuthorizedConnectionAsync(accountNo, role, callerId);
        if (connection is null) return null;

        return await GetPaymentsAsync(connection.ConnId);
    }

    // partner viewing their payments only
    public async Task<List<PaymentHistoryResponse>?> GetPartnerPaymentsAsync(int partnerId)
    {
        var partnerExists = await _context.Partners.AnyAsync(p => p.PartnerId == partnerId);
        if (!partnerExists) return null;

        return await _context.Payments
            .Where(p => p.PartnerId == partnerId)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new PaymentHistoryResponse
            {
                PaymentId = p.PaymentId,
                Date = p.PaymentDate,
                Amount = p.TotalPaid,
                Status = p.Status,
                BillId = p.BillId
            })
            .ToListAsync();
    }

    public async Task<ViewAccResponse?> GetAccountDetailsAsync(string accountNo, string role, int callerId, string? nic)
    {
        var connection = await GetAuthorizedConnectionAsync(accountNo, role, callerId);
        if (connection is null) return null;

        if (string.IsNullOrWhiteSpace(nic) ||
            !string.Equals(connection.Customer.NIC.Trim(), nic.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new NicMismatchException(); //  more specific than "not found"
        }

        return new ViewAccResponse
        {
            AccountNo = connection.AccNo,
            Address = connection.Address,
            Status = connection.Status
        };
    }

    private async Task<List<ViewMeterReading>> GetReadingsAsync(int connId)
    {
        return await _context.MeterReadings
            .Where(r => r.ConnId == connId)
            .OrderByDescending(r => r.ReadingDate)
            .Select(r => new ViewMeterReading
            {
                ReadingId = r.ReadingId,
                Date = r.ReadingDate,
                Value = r.CurrentValue
            })
            .ToListAsync();
    }

    private async Task<List<PaymentHistoryResponse>> GetPaymentsAsync(int connId)
    {
        return await _context.Payments
            .Where(p => p.Bill.ConnId == connId)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new PaymentHistoryResponse
            {
                PaymentId = p.PaymentId,
                Date = p.PaymentDate,
                Amount = p.TotalPaid,
                Status = p.Status,
                BillId = p.BillId
            })
            .ToListAsync();
    }

    public async Task<StaffViewAccResponse?> GetStaffAccountDetailsAsync(string accountNo)
    {
        var connection = await _context.WaterConnections
            .Include(w => w.Customer)
            .FirstOrDefaultAsync(w => w.AccNo == accountNo);

        if (connection is null) return null;

        return new StaffViewAccResponse
        {
            AccountNo = connection.AccNo,
            Address = connection.Address,
            Status = connection.Status,
            CustomerName = connection.Customer.FullName,
            ContactPhone = connection.Customer.PhoneNo,
            ContactEmail = connection.Customer.Email
        };
    }

    public async Task<CreateBillPreviewResponse?> GetBillPrepAsync(string accountNo)
    {
        var connection = await _context.WaterConnections
            .FirstOrDefaultAsync(w => w.AccNo == accountNo);

        if (connection is null) return null;

        var lastBill = await _context.Bills
            .Where(b => b.ConnId == connection.ConnId)
            .OrderByDescending(b => b.IssuedDate)
            .FirstOrDefaultAsync();

        var readingsQuery = _context.MeterReadings
            .Where(r => r.ConnId == connection.ConnId);

        if (lastBill is not null)
        {
            readingsQuery = readingsQuery.Where(r => r.ReadingDate >= lastBill.IssuedDate);
        }

        var readings = await readingsQuery
            .OrderByDescending(r => r.ReadingDate)
            .Select(r => new ViewMeterReading
            {
                ReadingId = r.ReadingId,
                Date = r.ReadingDate,
                Value = r.CurrentValue
            })
            .ToListAsync();

        return new CreateBillPreviewResponse
        {
            AccountNo = connection.AccNo,
            Status = connection.Status,
            Readings = readings
        };
    }

    public async Task<bool> AccountExistsAsync(string accountNo)
    {
        return await _context.WaterConnections
            .AnyAsync(w => w.AccNo == accountNo);
    }
}