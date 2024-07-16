using System.Net;
using FluentAssertions;
using LightBDD.XUnit2;
using PaymentGateway.ServiceTests.Infrastructure;

namespace PaymentGateway.ServiceTests;

public partial class HealthCheckTests : FeatureFixture, IDisposable
{
    private HttpResponseMessage _healthStatusResponse;

    private readonly HttpClient _client = ServiceWebApplicationFactory.Instance.CreateClient();

    private async Task Service_is_running()
    {
        await Task.CompletedTask;
    }

    private async Task Check_the_health_status()
    {
        _healthStatusResponse = await _client.GetAsync("/health");
    }

    private async Task The_response_should_be_healthy()
    {
        _healthStatusResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}