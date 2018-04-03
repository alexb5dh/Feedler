using System;
using System.Net;

namespace Feedler.Extensions.MVC.ExceptionHandling
{
    /// <summary>
    /// Base class for exception that contains info that should be presented to the API user.
    /// Should be used in conjunction with <see cref="ExceptionHandlingMiddleware"/>.
    /// </summary>
    /// <remarks>
    /// <example> <see cref="NotFoundException"/> uses 404 HTTP code to inform user that given entity is not found. </example>
    /// </remarks>
    [Serializable]
    public class HttpException: Exception
    {
        /// <summary>
        /// HTTP code that should be in response if this exception is raised.
        /// </summary>
        public HttpStatusCode HttpCode { get; set; } = HttpStatusCode.InternalServerError;

        /// <summary>
        /// Exception message to provide in response.
        /// </summary>
        public string Message { get; set; } = "Internal Server Error";

        /// <summary>
        /// Exception code to provide in response for use by programmed API clients.
        /// </summary>
        public string Code { get; set; }

        public HttpException(){}

        public HttpException(HttpStatusCode httpCode, string message, string code = null): base(message)
        {
            HttpCode = httpCode;
            Message = message;
            Code = code;
        }
    }
}