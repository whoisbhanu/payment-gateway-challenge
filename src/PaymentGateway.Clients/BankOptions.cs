namespace PaymentGateway.Clients;

public record BankOptions
{
    public required Uri Url { get; set; }
}