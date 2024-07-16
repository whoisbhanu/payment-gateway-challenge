using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Contracts.Requests;
using PaymentGateway.Contracts.Responses;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly PaymentsController _sut;
    private readonly IPaymentService _mockPaymentService;
    private readonly IFixture _fixture = new Fixture();
    private readonly ILogger<PaymentsController> _logger = new NullLogger<PaymentsController>();

    public PaymentsControllerTests()
    {
        _mockPaymentService = Substitute.For<IPaymentService>();
        _sut = new PaymentsController(_mockPaymentService, _logger);
    }

    [Fact]
    public void Get_Payment_By_Id_Should_Succeed()
    {
    }
    
    [Fact]
    public void Get_Payment_By_Id_Should_Return_Not_Found()
    {
    }
    
    [Fact]
    public async Task Process_Payment_Should_Succeed()
    {
        var request = _fixture.Create<PaymentRequest>();
        var expectedResponse = _fixture.Create<PaymentResponse>();

        _mockPaymentService.ProcessPayment(request).Returns(expectedResponse);

        var response = await _sut.ProcessPayment(request);
        
        response.Result
            .Should().BeOfType<OkObjectResult>()
            .And.Subject.As<OkObjectResult>()
            .Value.As<PaymentResponse>()
            .Should().Be(expectedResponse);
    }
}