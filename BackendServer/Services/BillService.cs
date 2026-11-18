using Microsoft.EntityFrameworkCore;
using NWSDB5013.Data;
using NWSDB5013.DTOs;
using NWSDB5013.Models;

namespace NWSDB5013.Services;

public class BillService : IBillService
{
    private readonly AppDbContext _context;

    public BillService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ViewBillResponse?> GenerateBillAsync(CreateBillRequest request)
    {
        var connection = await _context.WaterConnections
            .FirstOrDefaultAsync(w => w.AccNo == request.AccountNo);

        if (connection is null) return null;

        // first check if the staff exists since this is staff-only
        var staffExists = await _context.Staff.AnyAsync(s => s.StaffId == request.StaffId);
        if (!staffExists) return null;

        // find unpaid bill of the connection
        var hasUnpaidBill = await _context.Bills
            .AnyAsync(b => b.ConnId == connection.ConnId && b.Status == "Unpaid");
        if (hasUnpaidBill) return null;

        // find the last bill to find where the previous billing period ended
        var lastBill = await _context.Bills
            .Where(b => b.ConnId == connection.ConnId)
            .OrderByDescending(b => b.IssuedDate)
            .FirstOrDefaultAsync();

        // reading after the last bills issue date
        MeterReading? startReading;

        if (lastBill is not null)
        {
            startReading = await _context.MeterReadings
                .Where(r => r.ConnId == connection.ConnId && r.ReadingDate >= lastBill.IssuedDate)
                .OrderBy(r => r.ReadingDate)
                .FirstOrDefaultAsync();
        }
        else
        {
            // if there was no prev bill, use the first reading avaialble
            startReading = await _context.MeterReadings
                .Where(r => r.ConnId == connection.ConnId)
                .OrderBy(r => r.ReadingDate)
                .FirstOrDefaultAsync();
        }

        var endReading = await _context.MeterReadings
            .Where(r => r.ConnId == connection.ConnId)
            .OrderByDescending(r => r.ReadingDate)
            .FirstOrDefaultAsync();

        if (startReading is null || endReading is null || startReading.ReadingId == endReading.ReadingId)
            return null; // not enough reading history to get a period

        var unitsUsed = (int)(endReading.CurrentValue - startReading.CurrentValue);

        var bill = new Bill
        {
            IssuedDate = DateTime.UtcNow,
            DueDate = request.DueDate,
            TotalPayable = request.TotalPayable,
            UnitsUsed = unitsUsed,
            Status = "Unpaid",
            ConnId = connection.ConnId,
            StaffId = request.StaffId
        };

        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        return new ViewBillResponse
        {
            BillId = bill.BillId,
            IssuedDate = bill.IssuedDate,
            DueDate = bill.DueDate,
            TotalPayable = bill.TotalPayable,
            UnitsUsed = bill.UnitsUsed,
            Status = bill.Status
        };
    }
}