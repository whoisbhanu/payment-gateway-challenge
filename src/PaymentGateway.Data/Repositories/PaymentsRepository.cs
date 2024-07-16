namespace PaymentGateway.Data.Repositories;

public class PaymentsRepository : IPaymentsRepository
{
    private static List<PaymentDocument> s_payments;

    public PaymentsRepository()
    {
        s_payments = new List<PaymentDocument>
        {
            new PaymentDocument
            {
                Id = Guid.Parse("d9e93c73-5cd4-4126-ad89-42effdd05d6d"),
                Status = PaymentStatus.Declined,
                Amount = 600,
                Currency = "USD",
                Type = PaymentType.Card,
                CardDetails = new Card
                {
                    LastFourDigits = 7890,
                    ExpiryMonth = 1,
                    ExpiryYear = 27
                }
            }
        };
    }

    public Task<PaymentDocument> GetPayment(Guid id, CancellationToken token = default)
    {
        var matchedPayment = s_payments.FirstOrDefault(x => x.Id == id);

        if (matchedPayment != null)
            return Task.FromResult(matchedPayment);
        throw new PaymentNotFoundException();
    }

    public Task<PaymentDocument> Save(PaymentDocument paymentDocument, CancellationToken token = default)
    {
        s_payments.Add(paymentDocument);
        return GetPayment(paymentDocument.Id, token);
    }
}