using System;
using System.Threading.Tasks;
using Feedler.Extensions.MVC.ExceptionHandling.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Feedler.Extensions.MVC.ExceptionHandling
{
    /// <summary>
    /// Middleware processes all unhandled exception in subsequent handlers,
    /// serializes them to response and write to log.
    /// Details provided depends on whether the exception is <see cref="HttpException"/>
    /// and whether forwarding is set in options.
    /// Serialization happens via <see cref="JsonConverter"/>, content-type is 'application/json'.
    /// </summary>
    public sealed class ExceptionHandlingMiddleware
    {
        public class Options
        {
            public bool ForwardExceptions { get; set; } = false;
        }

        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly Options _options;

        public ExceptionHandlingMiddleware(RequestDelegate next, IHostingEnvironment environment, ILogger<ExceptionHandlingMiddleware> logger,
            Options options)
        {
            _next = next;
            _environment = environment;
            _logger = logger;
            _options = options;
        }

        public ExceptionHandlingMiddleware(RequestDelegate next, IHostingEnvironment environment, ILogger<ExceptionHandlingMiddleware> logger,
            IOptions<Options> options): this(next, environment, logger, options.Value) { }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpException exception)
            {
                await HandleHttpExceptionAsync(context, exception);
            }
            catch (Exception exception)
            {
                await HandleInternalException(context, exception);
            }
        }

        private async Task HandleHttpExceptionAsync(HttpContext context, HttpException exception)
        {
            _logger.LogWarning($"Handling {nameof(HttpException)} ({exception.GetType().Name}).\n" +
                               $"Request: {context.Request.Method} {context.Request.Path}\n" +
                               $"Message: {exception.Message}");

            await WriteErrorsResponse(context.Response, exception);
        }

        private async Task HandleInternalException(HttpContext context, Exception exception)
        {
            _logger.LogError($"Handling internal exception ({exception.GetType().Name}).\n" +
                             $"Request: {context.Request.Method} {context.Request.Path}\n" +
                             $"Exception: {exception}");

            await WriteErrorsResponse(context.Response,
                _options.ForwardExceptions
                    ? new HttpException(exception.Message)
                    : new HttpException());
        }

        private async Task WriteErrorsResponse(HttpResponse response, HttpException exception)
        {
            var responseContent = new { exception.Code, exception.Message };

            if (!response.HasStarted)
            {
                response.ContentType = "application/json";
                response.StatusCode = (int)exception.HttpCode;
                await response.WriteAsync(JsonConvert.SerializeObject(responseContent)).ConfigureAwait(false);
            }

            else
            {
                _logger.LogError($"Failed to write {responseContent} as response has already started.");
            }
        }
    }
}