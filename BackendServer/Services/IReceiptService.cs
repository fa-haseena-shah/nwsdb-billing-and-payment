using NWSDB5013.DTOs;

namespace NWSDB5013.Services;

public interface IReceiptService
{
    Task<ReceiptResponse?> GetReceiptAsync(int receiptId);
    Task SendReceiptEmailAsync(ReceiptResponse receipt, string toEmail);
}