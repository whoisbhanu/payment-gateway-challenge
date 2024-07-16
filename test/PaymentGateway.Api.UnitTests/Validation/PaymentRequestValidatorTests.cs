using PaymentGateway.Api.Validation;
using AutoFixture;
using FluentValidation.TestHelper;
using PaymentGateway.Contracts;
using PaymentGateway.Contracts.Requests;

namespace PaymentGateway.Api.Tests.Validation;

public class PaymentRequestValidatorTests
{
    private readonly Fixture _fixture;
    private readonly Card _validCardDetails;

    public PaymentRequestValidatorTests()
    {
        _fixture = new Fixture();
        _validCardDetails = new Card
        {
            CardNumber = "2222405343248112", ExpiryMonth = 01, ExpiryYear = 2026, Cvv = "123"
        };
    }

    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    public void PaymentRequestValidator_ValidCurrency_ShouldPassValidation(string currency)
    {
        var paymentRequest = _fixture.Build<PaymentRequest>()
            .With(r => r.Currency, currency)
            .With(r => r.CardDetails, _validCardDetails)
            .Create();

        var validator = new PaymentRequestValidator();

        var result = validator.TestValidate(paymentRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("CAD")]
    [InlineData("JPY")]
    public void PaymentRequestValidator_InvalidCurrency_ShouldFailValidation(string currency)
    {
        var paymentRequest = _fixture.Build<PaymentRequest>()
            .With(r => r.Currency, currency)
            .With(r => r.CardDetails, _validCardDetails)
            .Create();

        var validator = new PaymentRequestValidator();

        var result = validator.TestValidate(paymentRequest);

        result.ShouldHaveValidationErrorFor(r => r.Currency)
            .WithErrorMessage("Currency must be one of the allowed values: USD, EUR, GBP.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void PaymentRequestValidator_InvalidAmount_ShouldFailValidation(int amount)
    {
        var paymentRequest = _fixture.Build<PaymentRequest>()
            .With(r => r.CardDetails, _validCardDetails)
            .With(r => r.Amount, amount)
            .Create();

        var validator = new PaymentRequestValidator();

        var result = validator.TestValidate(paymentRequest);

        result.ShouldHaveValidationErrorFor(r => r.Amount)
            .WithErrorMessage("Amount should be greater than 0");
    }
}