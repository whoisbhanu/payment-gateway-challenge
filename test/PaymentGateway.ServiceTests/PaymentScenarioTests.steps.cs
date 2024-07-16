using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using PaymentGateway.Clients.Contracts;
using PaymentGateway.Contracts;
using PaymentGateway.Contracts.Requests;
using PaymentGateway.Contracts.Responses;
using PaymentGateway.Domain.Extensions;
using PaymentGateway.ServiceTests.Infrastructure;
using PaymentGateway.ServiceTests.Mocks;

namespace PaymentGateway.ServiceTests;

public partial class PaymentScenarioTests
{
    private readonly HttpClient _client = ServiceWebApplicationFactory.Instance.CreateClient();
    private HttpResponseMessage _response;
    private PaymentRequest _paymentRequest;
    private Guid _paymentId;
    private readonly JsonSerializerOptions _jsonSerializerSettings = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    private Task User_with_valid_card_and_enough_balance()
    {
        _paymentRequest = SetupPaymentRequest();

        var bankRequest = _paymentRequest.ToBankRequest();
        var bankResponse = new BankResponse { Authorized = true, AuthorizationCode = null };

        MockBankProvider.ConfigureProcessPayment(bankRequest, bankResponse );

        return Task.CompletedTask;
    }

    private async Task Payment_is_attempted()
    {
        var paymentPayload = JsonSerializer.Serialize(_paymentRequest);
        var request = new HttpRequestMessage(HttpMethod.Post, "payments");
        request.Content = new StringContent(paymentPayload, Encoding.UTF8, MediaTypeNames.Application.Json);
        _response = await _client.SendAsync(request);
    }

    private async Task Response_is_authorized()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await _response.Content.ReadAsStringAsync();
        var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(content, _jsonSerializerSettings);
        _paymentId = paymentResponse.Id;
        paymentResponse.Status.Should().Be(PaymentStatus.Authorized);
        paymentResponse.Amount.Should().Be(_paymentRequest.Amount);
        paymentResponse.Currency.Should().Be(_paymentRequest.Currency);
        var cardNumber = _paymentRequest.CardDetails.CardNumber;
        paymentResponse.LastFourCardDigits.Should().Be(cardNumber.Substring(cardNumber.Length-4));
        paymentResponse.ExpiryMonth.Should().Be(_paymentRequest.CardDetails.ExpiryMonth.ToString());
        paymentResponse.ExpiryYear.Should().Be(_paymentRequest.CardDetails.ExpiryYear.ToString());
    }

    private Task User_with_valid_card_and_not_enough_balance()
    {
        _paymentRequest = SetupPaymentRequest();

        var bankRequest = _paymentRequest.ToBankRequest();
        var bankResponse = new BankResponse { Authorized = false, AuthorizationCode = null };

        MockBankProvider.ConfigureProcessPayment(bankRequest, bankResponse );

        return Task.CompletedTask;
    }

    private async Task Response_is_declined()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await _response.Content.ReadAsStringAsync();
        var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(content, _jsonSerializerSettings);
        paymentResponse.Status.Should().Be(PaymentStatus.Declined);
    }

    private Task User_with_invalid_card()
    {
        _paymentRequest = SetupInvalidPaymentRequest();

        return Task.CompletedTask;
    }

    private Task Response_is_rejected()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        return Task.CompletedTask;
    }
    
    private static PaymentRequest SetupPaymentRequest()
    {
        return new PaymentRequest
        {
            Currency = "GBP",
            Amount = 10,
            Type = PaymentType.Card,
            CardDetails = new Card
            {
                CardNumber = "2222405343248112", ExpiryMonth = 01, ExpiryYear = 2026, Cvv = "456"
            }
        };
    }
    
    private static PaymentRequest SetupInvalidPaymentRequest()
    {
        return new PaymentRequest
        {
            Currency = "INR",
            Amount = 10,
            Type = PaymentType.Card,
            CardDetails = new Card
            {
                CardNumber = "AA2222405343248112", ExpiryMonth = 01, ExpiryYear = 2026, Cvv = "456"
            }
        };
    }

    private async Task User_made_a_payment()
    {
        await PaymentGateway_process_payment_endpoint_success();
    }

    private async Task Merchant_retrieves_the_payment()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"payments/{_paymentId}");
        _response = await _client.SendAsync(request);
    }

    private async Task Payment_response_is_provided()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await _response.Content.ReadAsStringAsync();
        var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(content, _jsonSerializerSettings);
        paymentResponse.Id.Should().Be(_paymentId);
        paymentResponse.Status.Should().Be(PaymentStatus.Authorized);
        paymentResponse.Amount.Should().Be(_paymentRequest.Amount);
        paymentResponse.Currency.Should().Be(_paymentRequest.Currency);
        var cardNumber = _paymentRequest.CardDetails.CardNumber;
        paymentResponse.LastFourCardDigits.Should().Be(cardNumber.Substring(cardNumber.Length-4));
        paymentResponse.ExpiryMonth.Should().Be(_paymentRequest.CardDetails.ExpiryMonth.ToString());
        paymentResponse.ExpiryYear.Should().Be(_paymentRequest.CardDetails.ExpiryYear.ToString());
    }

    private async Task Merchant_retrieves_non_existing_payment()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"payments/{Guid.NewGuid()}");
        _response = await _client.SendAsync(request);
    }

    private Task Payment_response_is_provided_with_not_found()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        return Task.CompletedTask;
    }
}