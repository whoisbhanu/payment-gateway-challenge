using PaymentGateway.Contracts.Requests;
using PaymentGateway.Contracts.Responses;

namespace PaymentGateway.Domain;

public interface IPaymentService
{
    Task<PaymentResponse> GetPayment(Guid id, CancellationToken token = default);

    Task<PaymentResponse> ProcessPayment(PaymentRequest request, CancellationToken token = default);
}