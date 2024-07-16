using Microsoft.Extensions.Configuration;

namespace PaymentGateway.ServiceTests.Infrastructure;

public static class Settings
{
    public static IConfiguration Configuration { get; }

    static Settings()
    {
        var appSettingsEnv = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.ci.json");
        var config = new ConfigurationBuilder()
            .AddJsonFile(appSettingsEnv, false, true)
            .Build();
        Configuration = config;
    }
}