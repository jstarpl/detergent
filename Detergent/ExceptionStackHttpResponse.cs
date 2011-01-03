using System;
using System.Net;

namespace Detergent
{
    public class ExceptionStackHttpResponse : LiteralHttpResponse
    {
        public ExceptionStackHttpResponse(HttpStatusCode statusCode, Exception ex) 
            : base(statusCode, ex.ToString(), HttpConstants.ContentTypeText)
        {
        }
    }
}