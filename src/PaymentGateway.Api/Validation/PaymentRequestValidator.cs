using FluentValidation;

using PaymentGateway.Contracts.Requests;

namespace PaymentGateway.Api.Validation;

public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
{
    private readonly HashSet<string> _isoCurrencyCodes = new HashSet<string>
    {
        "USD", "EUR", "GBP"
        // Add all ISO currency codes here
    };
    
    public PaymentRequestValidator()
    {
        RuleFor(r => r.CardDetails).SetValidator(new CardDetailsValidator())
            .WithMessage("Incorrect card details");
        RuleFor(r => r.Amount).GreaterThan(0)
            .WithMessage("Amount should be greater than 0");
        RuleFor(r => r.Type).IsInEnum()
            .WithMessage("Payment type should be from supported types");
        RuleFor(r => r.Currency).Must(BeInAcceptedCurrencies)
            .WithMessage("Currency must be one of the allowed values: USD, EUR, GBP.");
    }
    
    private bool BeInAcceptedCurrencies(string currencyCode)
    {
        return _isoCurrencyCodes.Contains(currencyCode);
    }
}