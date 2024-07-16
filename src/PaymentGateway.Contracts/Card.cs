using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Contracts;

public record Card
{
    [Required]
    public required string CardNumber { get; set; }

    [Required]
    public int ExpiryMonth { get; set; }
    
    [Required]
    public int ExpiryYear { get; set; }
    
    [Required]
    public string Cvv { get; set; }
}