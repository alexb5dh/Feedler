using System;
using System.Net;

namespace Feedler.Extensions.MVC.ExceptionHandling.Exceptions
{
    /// <summary>
    /// Base class for exception that contains info that should be presented to the API user.
    /// Should be used in conjunction with <see cref="ExceptionHandlingMiddleware"/>.
    /// </summary>
    /// <remarks>
    /// <example> <see cref="NotFoundException"/> uses <see cref="HttpStatusCode.NotFound"/> HTTP code to inform user that given entity is not found. </example>
    /// <example> <see cref="ConflictException"/> uses <see cref="HttpStatusCode.Conflict"/> HTTP code to inform user that given entity is already created. </example>
    /// </remarks>
    [Serializable]
    public class HttpException: Exception
    {
        /// <summary>
        /// HTTP code that should be in response if this exception is raised.
        /// </summary>
        public HttpStatusCode HttpCode { get; set; }

        /// <summary>
        /// Exception code to provide in response for use by programmed API clients.
        /// </summary>
        public string Code { get; set; }

        public HttpException(string message = "Internal Server Error"): base(message) { }
    }
}