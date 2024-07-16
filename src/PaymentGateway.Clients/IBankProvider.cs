using PaymentGateway.Clients.Contracts;

namespace PaymentGateway.Clients;

public interface IBankProvider
{
    Task<BankResponse> ProcessPayment(BankPaymentRequest request, CancellationToken cancellationToken);
}