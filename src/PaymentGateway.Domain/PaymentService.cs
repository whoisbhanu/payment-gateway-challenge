using PaymentGateway.Clients;
using PaymentGateway.Contracts.Requests;
using PaymentGateway.Contracts.Responses;
using PaymentGateway.Data;
using PaymentGateway.Data.Repositories;
using PaymentGateway.Domain.Extensions;

namespace PaymentGateway.Domain;

public class PaymentService : IPaymentService
{
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly IBankProvider _bankSimulator;

    public PaymentService(IPaymentsRepository paymentsRepository, IBankProvider bankSimulator)
    {
        _paymentsRepository = paymentsRepository;
        _bankSimulator = bankSimulator;
    }

    public async Task<PaymentResponse> GetPayment(Guid id, CancellationToken token)
    {
        var payment = await _paymentsRepository.GetPayment(id, token);
        var paymentResponse = payment.ToPaymentResponse();
        return paymentResponse;
    }

    public async Task<PaymentResponse> ProcessPayment(PaymentRequest request, CancellationToken token)
    {
        var bankRequest = request.ToBankRequest();
        var bankResponse = await _bankSimulator.ProcessPayment(bankRequest, token);
        var payment = new PaymentDocument
        {
            Id = Guid.NewGuid(),
            Status = bankResponse.Authorized ? Data.PaymentStatus.Authorized : Data.PaymentStatus.Declined,
            Amount = bankRequest.Amount,
            Currency = bankRequest.Currency,
            Type = PaymentType.Card,
            CardDetails = new Card
            {
                LastFourDigits = Convert.ToInt32(bankRequest.CardNumber.Substring(request.CardDetails.CardNumber.Length-4)),
                ExpiryMonth = request.CardDetails.ExpiryMonth,
                ExpiryYear = request.CardDetails.ExpiryYear
            }
        };
        var savedPayment = await _paymentsRepository.Save(payment, token);
        var response = savedPayment.ToPaymentResponse();
        return response;
    }
}