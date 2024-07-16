using FluentAssertions;
using PaymentGateway.Data.Repositories;

namespace PaymentGateway.Data.UnitTests;

public class PaymentRepositoryTests
{
    private readonly PaymentsRepository _sut = new();

    [Fact]
    public async Task GetPayment_Should_Return_Payment()
    {
        var paymentId = Guid.Parse("d9e93c73-5cd4-4126-ad89-42effdd05d6d");

        var paymentResult = await _sut.GetPayment(paymentId);

        paymentResult.Id.Should().Be(paymentId);
        paymentResult.Status.Should().Be(PaymentStatus.Declined);
        paymentResult.Amount.Should().Be(600);
        paymentResult.Currency.Should().Be("USD");
        paymentResult.Type.Should().Be(PaymentType.Card);
        paymentResult.CardDetails.ExpiryMonth.Should().Be(1);
        paymentResult.CardDetails.ExpiryYear.Should().Be(27);
        paymentResult.CardDetails.LastFourDigits.Should().Be(7890);
    }
    
    [Fact]
    public async Task GetPayment_Should_Throw_When_Not_Found()
    {
        var paymentId = Guid.NewGuid();

        Func<Task> act = () =>  _sut.GetPayment(paymentId);

        await act.Should().ThrowAsync<PaymentNotFoundException>();
    }
}