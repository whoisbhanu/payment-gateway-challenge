using FluentAssertions;
using PaymentGateway.Contracts;
using PaymentGateway.Contracts.Requests;
using PaymentGateway.Domain.Extensions;

namespace PaymentGateway.Domain.UnitTests.Extensions;

public class PaymentRequestExtensionsTests
{
    [Fact]
    public void ToBankRequest_ShouldMapPropertiesCorrectly()
    {
        var paymentRequest = new PaymentRequest
        {
            CardDetails = new Card
            {
                CardNumber = "1234567890123456",
                ExpiryMonth = 11,
                ExpiryYear = 2026,
                Cvv = "123"
            },
            Currency = "USD",
            Amount = 10050
        };

        var bankRequest = paymentRequest.ToBankRequest();

        bankRequest.CardNumber.Should().Be("1234567890123456");
        bankRequest.ExpiryDate.Should().Be("11/2026");
        bankRequest.Currency.Should().Be("USD");
        bankRequest.Amount.Should().Be(10050);
        bankRequest.Cvv.Should().Be("123");
    }

    [Fact]
    public void ToBankRequest_ShouldHandleSingleDigitMonthCorrectly()
    {
        var paymentRequest = new PaymentRequest
        {
            CardDetails = new Card
            {
                CardNumber = "1234567890123456",
                ExpiryMonth = 5,
                ExpiryYear = 2025,
                Cvv = "456"
            },
            Currency = "EUR",
            Amount = 10050
        };

        var bankRequest = paymentRequest.ToBankRequest();

        bankRequest.CardNumber.Should().Be("1234567890123456");
        bankRequest.ExpiryDate.Should().Be("05/2025");
        bankRequest.Currency.Should().Be("EUR");
        bankRequest.Amount.Should().Be(10050);
        bankRequest.Cvv.Should().Be("456");
    }
}