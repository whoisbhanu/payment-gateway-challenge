using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace PaymentGateway.ServiceTests;

[FeatureDescription(@"Set up payment gateway service")]
public partial class HealthCheckTests
{
    [Scenario]
    public async Task PaymentGateway_health_check_endpoint_success()
    {
        await Runner.RunScenarioAsync(given => Service_is_running(),
            when => Check_the_health_status(),
            then => The_response_should_be_healthy());
    }
}