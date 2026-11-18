using Microsoft.EntityFrameworkCore;
using NWSDB5013.Data;
using NWSDB5013.DTOs;
using NWSDB5013.Models;

namespace NWSDB5013.Services;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;
    private readonly IReceiptService _receiptService;

    public PaymentService(AppDbContext context, IReceiptService receiptService)
    {
        _context = context;
        _receiptService = receiptService;
    }

    // this helper method will obtain the payer's email to  triigger the email sending
    private async Task<string?> GetCallerEmailAsync(string role, int callerId)
    {
        if (role == "Customer")
            return (await _context.Customers.FindAsync(callerId))?.Email;

        if (role == "Partner")
            return (await _context.Partners.FindAsync(callerId))?.Email;

        return null;
    }

    public async Task<PaymentResponse?> PayBillAsync(PaymentRequest request)
    {
        // check if a payment for this bill already exists  using the idempotency key
        var existing = await _context.Payments
            .Include(p => p.Receipt)
            .FirstOrDefaultAsync(p => p.IdempotencyKey == request.IdempotencyKey);

        if (existing is not null)
        {
            return new PaymentResponse
            {
                PaymentId = existing.PaymentId,
                Date = existing.PaymentDate,
                Amount = existing.TotalPaid,
                Status = existing.Status,
                PgRef = existing.PGRef,
                ReceiptId = existing.Receipt?.ReceiptId ?? 0
            };
        }

        // load bill + connection for checking purpose
        var bill = await _context.Bills
            .Include(b => b.WaterConnection)
            .FirstOrDefaultAsync(b => b.BillId == request.BillId);

        if (bill is null) return null;

        if (bill.Status == "Paid") return null; // already paid

        if (request.Role == "Customer" && bill.WaterConnection.CustomerId != request.CallerId)
            return null; // unauthorized accc owner

        if (request.Role == "Partner" && bill.WaterConnection.Status != "Active")
            return null; // partner cannot pay for a closed account

        if (request.Role != "Customer" && request.Role != "Partner")
            return null;   // staff cant pay

        if (request.Amount != bill.TotalPayable)
            return null;   // reject incorrect payments for this simplified flow

        // simulating payment gateway
        var (success, pgRef) = SimulateGatewayCharge(request.Amount);

        if (!success)
        {
            return new PaymentResponse
            {
                PaymentId = 0,
                Amount = request.Amount,
                Status = "Failed",
                PgRef = pgRef
            };
        }

        // sucess = create payment record, update bill status and generate receipt
        var payment = new Payment
        {
            PaymentDate = DateTime.UtcNow,
            TotalPaid = request.Amount,
            Status = "Success",
            PGRef = pgRef,
            IdempotencyKey = request.IdempotencyKey,
            BillId = bill.BillId,
            CustomerId = request.Role == "Customer" ? request.CallerId : null,
            PartnerId = request.Role == "Partner" ? request.CallerId : null
        };

        _context.Payments.Add(payment);
        bill.Status = "Paid";

        await _context.SaveChangesAsync();

        var receipt = new Receipt { PaymentId = payment.PaymentId };
        _context.Receipts.Add(receipt);
        await _context.SaveChangesAsync();

        // build receipt response + trigger email
        var receiptResponse = await _receiptService.GetReceiptAsync(receipt.ReceiptId);
        if (receiptResponse is not null)
        {
            var customerEmail = await GetCallerEmailAsync(request.Role, request.CallerId);
            if (!string.IsNullOrWhiteSpace(customerEmail))
                await _receiptService.SendReceiptEmailAsync(receiptResponse, customerEmail);
        }

        return new PaymentResponse
        {
            PaymentId = payment.PaymentId,
            Date = payment.PaymentDate,
            Amount = payment.TotalPaid,
            Status = payment.Status,
            PgRef = payment.PGRef,
            ReceiptId = receipt.ReceiptId
        };
    }

    // this function simulates the payment gateway payment processing 
    private (bool success, string pgRef) SimulateGatewayCharge(float amount)
    {
        var fakeRef = $"SIM-{Guid.NewGuid().ToString("N")[..10].ToUpper()}";
        return (true, fakeRef);
    }
}