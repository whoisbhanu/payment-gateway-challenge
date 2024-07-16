using System.Net;
using System.Net.Mime;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PaymentGateway.Clients.Contracts;
using RichardSzalay.MockHttp;

namespace PaymentGateway.Clients.UnitTests;

public class BankSimulatorTests
{
    private readonly IBankProvider _sut;
    private readonly MockHttpMessageHandler _mockHttp = new();
    private readonly IFixture _fixture = new Fixture();
    private readonly ILogger<BankSimulator> _logger = new NullLogger<BankSimulator>();
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
    private readonly string _baseUrl = "https://test-bank-simulator.com";
    
    public BankSimulatorTests()
    {
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(_baseUrl);
        _sut = new BankSimulator(httpClient, _logger);
    }

    [Fact]
    public async Task ProcessPayment_Should_Succeed()
    {
        var paymentRequest = _fixture.Create<BankPaymentRequest>();
        var expectedResponse = new BankResponse { Authorized = true, AuthorizationCode = "auth-code" };

        _mockHttp.When(HttpMethod.Post, $"{_baseUrl}/payments")
            .WithContent(JsonSerializer.Serialize(paymentRequest, _jsonSerializerOptions))
            .Respond(MediaTypeNames.Application.Json, JsonSerializer.Serialize(expectedResponse, _jsonSerializerOptions));

        var actualResponse = await _sut.ProcessPayment(paymentRequest, CancellationToken.None);
        actualResponse.Should().Be(expectedResponse);
    }
    
    [Fact]
    public async Task ProcessPayment_Should_Throw_ExternalHttpException_On_Failure()
    {
        var paymentRequest = _fixture.Create<BankPaymentRequest>();

        _mockHttp.When(HttpMethod.Post, $"{_baseUrl}/payments")
            .WithContent(JsonSerializer.Serialize(paymentRequest, _jsonSerializerOptions))
            .Respond(HttpStatusCode.BadRequest);

        var action = () => _sut.ProcessPayment(paymentRequest, CancellationToken.None);
        await action.Should().ThrowAsync<ExternalHttpRequestException>();
    }
}