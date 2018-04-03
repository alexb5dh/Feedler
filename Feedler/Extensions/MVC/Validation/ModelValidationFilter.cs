using Feedler.Extensions.MVC.ExceptionHandling.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Feedler.Extensions.MVC.Validation
{
    public class ModelValidationFilter: IActionFilter, IOrderedFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
                throw new BadRequestException(context.ModelState);
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public int Order => 100;
    }
}