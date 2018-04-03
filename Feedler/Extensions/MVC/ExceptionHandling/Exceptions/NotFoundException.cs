using System;
using System.Net;

namespace Feedler.Extensions.MVC.ExceptionHandling.Exceptions
{
    [Serializable]
    public class NotFoundException: HttpException
    {
        public NotFoundException(string message): base(message)
        {
            HttpCode = HttpStatusCode.NotFound;
        }
    }

    [Serializable]
    public class NotFoundException<TModel>: NotFoundException
    {
        public NotFoundException(): base($"{typeof(TModel).Name} not found.") { }

        public NotFoundException(string id): base($"{typeof(TModel).Name} #{id} not found.") { }
    }
}