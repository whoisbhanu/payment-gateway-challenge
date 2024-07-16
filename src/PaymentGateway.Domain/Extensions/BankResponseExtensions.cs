using PaymentGateway.Contracts.Responses;
using PaymentGateway.Data;

using PaymentStatus = PaymentGateway.Contracts.Responses.PaymentStatus;

namespace PaymentGateway.Domain.Extensions;

public static class BankResponseExtensions
{
    public static PaymentResponse ToPaymentResponse(this PaymentDocument savedPaymentDocument)
    {
        return new PaymentResponse
        {
            Id = savedPaymentDocument.Id,
            Status = (PaymentStatus)savedPaymentDocument.Status,
            Amount = savedPaymentDocument.Amount,
            Currency = savedPaymentDocument.Currency,
            LastFourCardDigits = savedPaymentDocument.CardDetails.LastFourDigits.ToString(),
            ExpiryMonth = savedPaymentDocument.CardDetails.ExpiryMonth.ToString(),
            ExpiryYear = savedPaymentDocument.CardDetails.ExpiryYear.ToString()
        };
    }
}