using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Contracts.Requests;
using PaymentGateway.Contracts.Responses;
using PaymentGateway.Data;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Controllers;

[ApiController]
[Route("payments")]
public class PaymentsController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentResponse>> GetById([FromRoute, Required] Guid id)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["ActionName"] = nameof(GetById), ["PaymentId"] = id
               }))
            try
            {
                _logger.LogInformation("Processing request to get payment");
                var response = await _paymentService.GetPayment(id);
                return Ok(response);
            }
            catch (PaymentNotFoundException e)
            {
                _logger.LogError("Failed to retrieve payment. Message - {Error}", e.Message);
                return NotFound();
            }
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PaymentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentResponse>> ProcessPayment([FromBody] PaymentRequest request)
    {
        using (_logger.BeginScope(new Dictionary<string, object> { ["ActionName"] = nameof(ProcessPayment) }))
            _logger.LogInformation("Processing request to make payment");

        var response = await _paymentService.ProcessPayment(request);
        return Ok(response);
    }
}