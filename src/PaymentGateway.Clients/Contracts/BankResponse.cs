namespace PaymentGateway.Clients.Contracts;

public record BankResponse
{
    public bool Authorized { get; set; }
    
    public string? AuthorizationCode { get; set; }
}