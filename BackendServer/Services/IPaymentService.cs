using NWSDB5013.DTOs;

namespace NWSDB5013.Services;

public interface IPaymentService
{
    Task<PaymentResponse?> PayBillAsync(PaymentRequest request);
}