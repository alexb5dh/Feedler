using System;
using System.Net;

namespace Feedler.Extensions.MVC.ExceptionHandling
{
    public class NotFoundException: HttpException
    {
        public NotFoundException(string message, string code = null):
            base(HttpStatusCode.NotFound, message, code) { }

        public NotFoundException(Type modelType, string id):
            this($"{modelType.Name} #{id} not found.") { }

        public NotFoundException(Type modelType, Guid id):
            this($"{modelType.Name} #{id} not found.") { }
    }
}