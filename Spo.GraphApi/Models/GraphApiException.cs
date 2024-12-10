using System.Net;

namespace Spo.GraphApi.Models;


public class GraphApiException : Exception
{
    public HttpStatusCode HttpStatusCode { get; }

    public string ErrorMessage { get; }

    public GraphApiException(HttpStatusCode httpStatusCode, string errorMessage)
    {
        HttpStatusCode = httpStatusCode;
        ErrorMessage = errorMessage;
    }
}
