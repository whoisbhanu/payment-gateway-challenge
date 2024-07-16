using System.Net;

namespace PaymentGateway.Clients;

public class ExternalHttpRequestException : Exception
{
    public HttpStatusCode StatusCode;

    public ExternalHttpRequestException(string errorMessage, HttpStatusCode responseStatusCode) : base(errorMessage)
    {
        StatusCode = responseStatusCode;
    }
}