using System;
using System.Net;

namespace Feedler.Extensions.MVC.ExceptionHandling.Exceptions
{
    [Serializable]
    public class ConflictException: HttpException
    {
        public ConflictException(string message = "Entity is already created."): base(message)
        {
            HttpCode = HttpStatusCode.Conflict;
        }
    }

    [Serializable]
    public class ConflictException<TModel>: ConflictException
    {
        public ConflictException(string id):
            base($"{typeof(TModel).Name} #{id} is already created.") { }

        public ConflictException():
            base($"{typeof(TModel).Name} is already created.") { }
    }
}