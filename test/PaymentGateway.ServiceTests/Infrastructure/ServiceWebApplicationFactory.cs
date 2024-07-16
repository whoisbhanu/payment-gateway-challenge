using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

using PaymentGateway.Api;

namespace PaymentGateway.ServiceTests.Infrastructure;

public class ServiceWebApplicationFactory : WebApplicationFactory<Program>
{
    private static Exception s_exception;
    private static ServiceWebApplicationFactory s_instance;

    private ServiceWebApplicationFactory()
    {
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("ci");
        return base.CreateHost(builder);
    }
    
    public static ServiceWebApplicationFactory Instance
    {
        get
        {
            if (s_instance is null)
            {
                s_instance = new ServiceWebApplicationFactory();
            }
    
            return s_instance;
        }
    }

    public static void Terminate(Action<IServiceProvider> terminationActions)
    {
        if (s_instance is null)
            return;

        terminationActions(s_instance.Services);
        s_instance.Dispose();
    }
}
