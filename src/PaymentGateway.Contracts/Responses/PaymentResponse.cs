namespace PaymentGateway.Contracts.Responses;

public record PaymentResponse
{
    public PaymentStatus Status { get; set; }
    public int Amount { get; set; }
    public string LastFourCardDigits { get; set; }
    public Guid Id { get; set; }
    public string ExpiryMonth { get; set; }
    public string ExpiryYear { get; set; }
    public string Currency { get; set; }
    
}