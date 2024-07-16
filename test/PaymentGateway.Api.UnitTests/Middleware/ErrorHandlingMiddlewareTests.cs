using System.Net;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using PaymentGateway.Api.Middleware;
using PaymentGateway.Clients;
using FluentAssertions;

using Microsoft.Extensions.Logging.Abstractions;

namespace PaymentGateway.Api.Tests.Middleware;

public class ErrorHandlingMiddlewareTests
{
    private readonly RequestDelegate _nextDelegate;
    private readonly HttpContext _httpContext;
    private readonly ErrorHandlingMiddleware _middleware;

    public ErrorHandlingMiddlewareTests()
    {
        _nextDelegate = Substitute.For<RequestDelegate>();
        var logger = new NullLogger<ErrorHandlingMiddleware>();
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
        _middleware = new ErrorHandlingMiddleware(_nextDelegate, logger);
    }

    [Fact]
    public async Task InvokeAsync_Should_Call_Next_Delegate_When_No_Exception()
    {
        _nextDelegate.Invoke(_httpContext).Returns(Task.CompletedTask);

        await _middleware.InvokeAsync(_httpContext);

        await _nextDelegate.Received(1).Invoke(_httpContext);
    }

    [Fact]
    public async Task InvokeAsync_Should_Handle_ExternalHttpRequestException()
    {
        var exception = new ExternalHttpRequestException("External request failed", HttpStatusCode.BadRequest);
        _nextDelegate.Invoke(_httpContext).Returns(Task.FromException(exception));

        await _middleware.InvokeAsync(_httpContext);

        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(_httpContext.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        responseBody.Should().Contain("Not supported by bank");
    }

    [Fact]
    public async Task InvokeAsync_Should_Handle_General_Exception()
    {
        var exception = new Exception("Something went wrong");
        _nextDelegate.Invoke(_httpContext).Returns(Task.FromException(exception));

        await _middleware.InvokeAsync(_httpContext);

        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(_httpContext.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        responseBody.Should().Be($"Something went wrong. Status Code : {(int)HttpStatusCode.InternalServerError}");
    }
}