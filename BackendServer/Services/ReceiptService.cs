using Microsoft.EntityFrameworkCore;
using NWSDB5013.Data;
using NWSDB5013.DTOs;

namespace NWSDB5013.Services;

public class ReceiptService : IReceiptService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public ReceiptService(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<ReceiptResponse?> GetReceiptAsync(int receiptId)
    {
        var receipt = await _context.Receipts
            .Include(r => r.Payment)
                .ThenInclude(p => p.Bill)
                    .ThenInclude(b => b.WaterConnection)
            .FirstOrDefaultAsync(r => r.ReceiptId == receiptId);

        if (receipt is null) return null;

        return new ReceiptResponse
        {
            ReceiptId = receipt.ReceiptId,
            PaymentId = receipt.PaymentId,
            PaymentDate = receipt.Payment.PaymentDate,
            Amount = receipt.Payment.TotalPaid,
            BillId = receipt.Payment.BillId,
            AccountNo = receipt.Payment.Bill.WaterConnection.AccNo
        };
    }

    public async Task SendReceiptEmailAsync(ReceiptResponse receipt, string toEmail)
    {
        var subject = $"NWSDB Payment Receipt #{receipt.ReceiptId}";

        var body = $"""
            Dear Customer,

            Thank you for your water connection payment. Please find your receipt below:

            Receipt ID: {receipt.ReceiptId}
            Account No: {receipt.AccountNo}
            Bill ID: {receipt.BillId}
            Payment Date: {receipt.PaymentDate:dd MMM yyyy}
            Amount Paid: Rs. {receipt.Amount:F2}

            This is an automated message from the Sri Lankan National Water Supply and Drainage Board.

            Thank you.
            """;

        await _emailService.SendEmailAsync(toEmail, subject, body);
    }
}