namespace PaymentGateway.Data;

public class PaymentDocument
{
    public required Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public int Amount { get; set; }
    public required string Currency { get; set; }
    public PaymentType Type { get; set; }
    public required Card CardDetails { get; set; }
}