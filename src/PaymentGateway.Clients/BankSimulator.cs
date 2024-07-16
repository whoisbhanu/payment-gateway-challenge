using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PaymentGateway.Clients.Contracts;

namespace PaymentGateway.Clients;

public class BankSimulator : IBankProvider
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
    private readonly ILogger<BankSimulator> _logger;

    public BankSimulator(HttpClient client, ILogger<BankSimulator> logger)
    {
        _client = client;
        _logger = logger;
    }
    public async Task<BankResponse> ProcessPayment(BankPaymentRequest paymentRequest, CancellationToken cancellationToken)
    {
        var paymentPayload = JsonSerializer.Serialize(paymentRequest, _jsonSerializerOptions);
        var request = new HttpRequestMessage(HttpMethod.Post, "payments");
        request.Content = new StringContent(paymentPayload, Encoding.UTF8);
        var response = await _client.SendAsync(request, cancellationToken);
       
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Error from simulator -{Error}, status code - {StatusCode}", errorMessage, response.StatusCode);
            throw new ExternalHttpRequestException(errorMessage, response.StatusCode);
        }
        
        var content = await response.Content.ReadAsStreamAsync(cancellationToken);
        var paymentResponse = JsonSerializer.Deserialize<BankResponse>(content, _jsonSerializerOptions);
        return paymentResponse;
    }
}