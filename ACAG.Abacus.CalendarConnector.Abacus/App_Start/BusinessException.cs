using System;
using System.Net;

namespace ACAG.Abacus.CalendarConnector.Abacus
{
    public class BusinessException : Exception
    {
        public BusinessException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
        
        public HttpStatusCode StatusCode { get; private set; }
    }
}