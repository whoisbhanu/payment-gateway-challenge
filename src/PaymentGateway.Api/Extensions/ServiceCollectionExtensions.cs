using FluentValidation;
using FluentValidation.AspNetCore;
using PaymentGateway.Api.Validation;
using PaymentGateway.Clients;
using PaymentGateway.Contracts.Requests;
using PaymentGateway.Data.Repositories;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        AddDomainDependencies(services);
        AddDataDependencies(services);
        AddExternalDependencies(services, configuration);
    }
    
    public static void AddValidationConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IValidator<PaymentRequest>, PaymentRequestValidator>();
        services.AddScoped<ValidationFilter>();
        services.AddFluentValidationAutoValidation();
    }

    private static void AddExternalDependencies(IServiceCollection services, IConfiguration configuration)
    {
        var bankOptions = configuration.GetSection("BankSimulator").Get<BankOptions>();
        services.AddHttpClient<IBankProvider, BankSimulator>("bank-simulator",
            (builder) =>
            {
                builder.BaseAddress = bankOptions?.Url;
            });
    }

    private static void AddDataDependencies(IServiceCollection services)
    {
        services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
    }

    private static void AddDomainDependencies(IServiceCollection services)
    {
        services.AddScoped<IPaymentService, PaymentService>();
    }
}