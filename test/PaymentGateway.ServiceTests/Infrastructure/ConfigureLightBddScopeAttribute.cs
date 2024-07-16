using LightBDD.XUnit2;

[assembly:PaymentGateway.ServiceTests.Infrastructure.ConfigureLightBddScope]
namespace PaymentGateway.ServiceTests.Infrastructure;

public class ConfigureLightBddScopeAttribute : LightBddScopeAttribute
{
    protected override void OnSetUp()
    {
        _ = ServiceWebApplicationFactory.Instance.CreateClient();
    }

    protected override void OnTearDown()
    {
        ServiceWebApplicationFactory.Terminate(serviceProvider =>
        {
            Console.Out.WriteLine($"Test termination finished at {DateTime.Now}");
        });
    }
    
    
}