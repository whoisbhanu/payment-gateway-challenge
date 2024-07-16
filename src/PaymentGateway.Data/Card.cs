namespace PaymentGateway.Data;

public class Card
{
    public int LastFourDigits { get; set;}
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
}