using PaymentGateway.Clients.Contracts;
using PaymentGateway.Contracts.Requests;

namespace PaymentGateway.Domain.Extensions;

public static class PaymentRequestExtensions
{
    public static BankPaymentRequest ToBankRequest(this PaymentRequest request)
    {
        return new BankPaymentRequest
        {
            CardNumber = request.CardDetails.CardNumber,
            ExpiryDate = $"{request.CardDetails.ExpiryMonth.ToString("00")}/{request.CardDetails.ExpiryYear}",
            Currency = request.Currency,
            Amount = request.Amount,
            Cvv = request.CardDetails.Cvv.ToString()
        };
    }
}