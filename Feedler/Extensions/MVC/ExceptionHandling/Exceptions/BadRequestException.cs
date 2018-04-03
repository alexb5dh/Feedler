using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Feedler.Extensions.MVC.ExceptionHandling.Exceptions
{
    [Serializable]
    public class BadRequestException: HttpException
    {
        public const string ModelErrorCode = "ModelError";

        public BadRequestException(string message, string code = null): base(message)
        {
            HttpCode = HttpStatusCode.BadRequest;
            Code = code;
        }

        public BadRequestException(ModelStateDictionary modelState): this(
            message: string.Join("\n", modelState.SelectMany(pair => pair.Value.Errors, (pair, error) => $"\"{pair.Key}\": {error.ErrorMessage}")),
            code: ModelErrorCode) { }
    }
}