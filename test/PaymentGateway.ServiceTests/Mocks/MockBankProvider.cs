using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PaymentGateway.Clients;
using PaymentGateway.Clients.Contracts;
using PaymentGateway.ServiceTests.Infrastructure;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace PaymentGateway.ServiceTests.Mocks;

public class MockBankProvider
{
    private static readonly WireMockServer _mockServer;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
    
    static MockBankProvider()
    {
        var bankUrl = Settings.Configuration.GetSection("BankSimulator").Get<BankOptions>().Url.ToString();
        _mockServer =  WireMockServer.Start(new WireMockServerSettings
        {
            Urls = new [] { bankUrl }
        });
        
    }
    
    public static void ConfigureProcessPayment(BankPaymentRequest request, BankResponse response)
    {
        _mockServer
            .Given(Request.Create()
                .UsingPost()
                .WithBodyAsJson(JsonSerializer.Serialize(request, _jsonSerializerOptions))
                .WithPath("/payments"))
            .RespondWith(Response.Create()
                .WithBody(JsonSerializer.Serialize(response, _jsonSerializerOptions))
                .WithStatusCode(HttpStatusCode.OK));
    }
}