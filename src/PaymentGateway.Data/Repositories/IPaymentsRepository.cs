namespace PaymentGateway.Data.Repositories;

public interface IPaymentsRepository
{
    Task<PaymentDocument> GetPayment(Guid id, CancellationToken token = default);
    Task<PaymentDocument> Save(PaymentDocument paymentDocument, CancellationToken token = default);
}