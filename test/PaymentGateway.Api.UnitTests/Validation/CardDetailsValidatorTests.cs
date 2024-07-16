using AutoFixture;
using FluentValidation.TestHelper;
using PaymentGateway.Api.Validation;
using PaymentGateway.Contracts;

namespace PaymentGateway.Api.Tests.Validation;

public class CardDetailsValidatorTests
{
    private readonly Fixture _fixture;

    public CardDetailsValidatorTests()
    {
        _fixture = new Fixture();
    }

    [Theory]
    [InlineData("")]
    [InlineData("12344")]
    [InlineData("411111111111111111111")]
    [InlineData("1234abcd56781314")]
    [InlineData("4111111111111111")]
    public void CardDetailsValidator_CardNumberValidation(string cardNumber)
    {
        var cardDetails = _fixture.Build<Card>()
            .With(c => c.CardNumber, cardNumber)
            .Create();

        var validator = new CardDetailsValidator();

        var result = validator.TestValidate(cardDetails);
        
        if (cardNumber == string.Empty)
        {
            result.ShouldHaveValidationErrorFor(c => c.CardNumber)
                .WithErrorMessage("'Card Number' must not be empty.");
        }

        else if (cardNumber.Length < 14)
        {
            result.ShouldHaveValidationErrorFor(c => c.CardNumber)
                .WithErrorMessage("The length of 'Card Number' must be at least 14 characters. You entered 5 characters.");
        }
        else if (cardNumber.Length > 19)
        {
            result.ShouldHaveValidationErrorFor(c => c.CardNumber)
                .WithErrorMessage("The length of 'Card Number' must be 19 characters or fewer. You entered 21 characters.");
        }
        else if (!cardNumber.All(char.IsDigit))
        {
            result.ShouldHaveValidationErrorFor(c => c.CardNumber)
                .WithErrorMessage("Card number has non-numeric character");
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(c => c.CardNumber);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(13)]
    public void CardDetailsValidator_ExpiryMonthValidation(int expiryMonth)
    {
        var cardDetails = _fixture.Build<Card>()
            .With(c => c.ExpiryMonth, expiryMonth)
            .Create();

        var validator = new CardDetailsValidator();

        var result = validator.TestValidate(cardDetails);

        if (expiryMonth is < 1 or > 12)
        {
            result.ShouldHaveValidationErrorFor(c => c.ExpiryMonth)
                .WithErrorMessage("Expiry month should be between 1-12");
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(c => c.ExpiryMonth);
        }
    }

    [Theory]
    [InlineData(2025)]
    [InlineData(2020)]
    public void CardDetailsValidator_ExpiryYearValidation(int expiryYear)
    {
        var cardDetails = _fixture.Build<Card>()
            .With(c => c.ExpiryYear, expiryYear)
            .Create();

        var validator = new CardDetailsValidator();

        var result = validator.TestValidate(cardDetails);

        if (expiryYear < DateTime.UtcNow.Year)
        {
            result.ShouldHaveValidationErrorFor(c => c.ExpiryYear)
                .WithErrorMessage("Expiry Year is in past");
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(c => c.ExpiryYear);
        }
    }

    [Theory]
    [InlineData(6, 2025)]
    [InlineData(1, 2022)]
    public void CardDetailsValidator_ExpiryDateValidation(int expiryMonth, int expiryYear)
    {
        var cardDetails = _fixture.Build<Card>()
            .With(c => c.ExpiryMonth, expiryMonth)
            .With(c => c.ExpiryYear, expiryYear)
            .Create();

        var validator = new CardDetailsValidator();

        var result = validator.TestValidate(cardDetails);
        
        var expiryDate = new DateTime(expiryYear, expiryMonth, 1);
        if (expiryDate < DateTime.UtcNow)
        {
            result.ShouldHaveValidationErrorFor(c => $"{c.ExpiryMonth:00}/{c.ExpiryYear}")
                .WithErrorMessage("Expiry month and year should be in future");
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(c => $"{c.ExpiryMonth:00}/{c.ExpiryYear}");
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("12a")]
    [InlineData("123")]
    [InlineData("1234")]
    public void CardDetailsValidator_CvvValidation(string cvv)
    {
        var cardDetails = _fixture.Build<Card>()
            .With(c => c.Cvv, cvv)
            .Create();

        var validator = new CardDetailsValidator();

        var result = validator.TestValidate(cardDetails);

        if (cvv.Length < 3 || cvv.Length > 4 || !cvv.All(char.IsDigit))
        {
            result.ShouldHaveValidationErrorFor(c => c.Cvv)
                .WithErrorMessage("Must be 3-4 numeric characters");
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(c => c.Cvv);
        }
    }

}