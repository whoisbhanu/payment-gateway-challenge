using System.Globalization;
using FluentValidation;
using PaymentGateway.Contracts;

namespace PaymentGateway.Api.Validation;

public class CardDetailsValidator : AbstractValidator<Card>
{
    public CardDetailsValidator()
    {
        RuleFor(c => c.CardNumber).NotEmpty().MinimumLength(14).MaximumLength(19);
        RuleFor(c => c.CardNumber).Must(BeNumeric)
            .WithMessage("Card number has non-numeric character");
        RuleFor(c => c.ExpiryMonth).InclusiveBetween(01,12)
            .WithMessage("Expiry month should be between 1-12");
        RuleFor(c => c.ExpiryYear).GreaterThanOrEqualTo(DateTime.UtcNow.Year)
            .WithMessage("Expiry Year is in past");;
        RuleFor(c => $"{c.ExpiryMonth:00}/{c.ExpiryYear}").Must(BeInFuture)
            .WithMessage("Expiry month and year should be in future");;
        RuleFor(c => c.Cvv).MinimumLength(3).MaximumLength(4).Must(BeNumeric)
            .WithMessage("Must be 3-4 numeric characters");;
    }
    
    private bool BeNumeric(string value)
    {
        return long.TryParse(value, out _);
    }
    
    private static bool BeInFuture(string value)
    {
        if (DateTime.TryParseExact(value, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month+1, 1);
            return firstDayOfMonth > DateTime.UtcNow;
        }
        return false;
    }
}