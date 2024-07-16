using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Contracts.Requests;

public record PaymentRequest
{
    [Required]
    public required string Currency { get; init; }
    
    [Required]
    public int Amount { get; init; }

    [Required]
    public PaymentType Type { get; set; }
    
    [Required]
    public required Card CardDetails { get; init; }
}