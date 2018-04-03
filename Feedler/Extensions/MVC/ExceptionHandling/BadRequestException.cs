using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Feedler.Extensions.MVC.ExceptionHandling
{
    public class BadRequestException: HttpException
    {
        public const string ModelErrorCode = "ModelError";

        public BadRequestException(string message, string code = null):
            base(HttpStatusCode.BadRequest, message, code) { }

        public BadRequestException(ModelStateDictionary modelState): base(
            HttpStatusCode.BadRequest,
            message: string.Join("\n", modelState.SelectMany(pair => pair.Value.Errors, (pair, error) => $"{pair.Key}: {error.ErrorMessage}.")),
            code: ModelErrorCode) { }
    }
}