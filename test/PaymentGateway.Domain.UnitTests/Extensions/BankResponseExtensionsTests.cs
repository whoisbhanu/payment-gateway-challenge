using FluentAssertions;
using PaymentGateway.Data;
using PaymentGateway.Domain.Extensions;

namespace PaymentGateway.Domain.UnitTests.Extensions;

public class BankResponseExtensionsTests
{
    [Fact]
    public void ToPaymentResponse_Should_Convert_Payment_To_Authorized_PaymentResponse()
    {
        var payment = new PaymentDocument
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Authorized,
            Amount = 100,
            Currency = "USD",
            CardDetails = new Card
            {
                LastFourDigits = 1222,
                ExpiryMonth = 01,
                ExpiryYear = 2025
            }
        };

        var paymentResponse = payment.ToPaymentResponse();

        paymentResponse.Currency.Should().Be(payment.Currency);
        paymentResponse.Status.Should().Be(Contracts.Responses.PaymentStatus.Authorized);
        paymentResponse.Id.Should().Be(payment.Id);
        paymentResponse.Amount.Should().Be(payment.Amount);
    }
    
    [Fact]
    public void ToPaymentResponse_Should_Convert_Payment_To_Declined_PaymentResponse()
    {
        var payment = new PaymentDocument
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Declined,
            Amount = 100,
            Currency = "USD",
            CardDetails = new Card
            {
                LastFourDigits = 1222,
                ExpiryMonth = 01,
                ExpiryYear = 2025
            }
        };

        var paymentResponse = payment.ToPaymentResponse();

        paymentResponse.Currency.Should().Be(payment.Currency);
        paymentResponse.Status.Should().Be(Contracts.Responses.PaymentStatus.Declined);
        paymentResponse.Id.Should().Be(payment.Id);
        paymentResponse.Amount.Should().Be(payment.Amount);
    }
}