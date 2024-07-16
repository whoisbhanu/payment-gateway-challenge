using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.Middleware;

namespace PaymentGateway.Api;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddServiceDependencies(builder.Configuration);
        builder.Services.AddHealthChecks();
        builder.Services.AddValidationConfiguration();
        builder.Configuration.AddUserSecrets(typeof(Program).Assembly);

        var app = builder.Build();

// Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        app.MapHealthChecks("/health");
        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.Run();
    }
}