using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Feedler.Extensions.MVC.Validation
{
    /// <summary>
    /// Validates all the validation attributes placed directly on action method parameters.
    /// </summary>
    /// <remarks>
    /// The framework by default doesn't evaluate the validation attributes put directly on action method parameters. It only evaluates the attributes put on the properties of the model types.
    /// This filter validates the attributes placed directly on action method parameters, and adds all the validation errors to the ModelState collection.
    /// </remarks>
    // https://github.com/markvincze/rest-api-helpers/blob/master/src/RestApiHelpers/Validation/ValidateActionParametersAttribute.cs
    public class ValidateActionParametersAttribute: ActionFilterAttribute
    {
        public ValidateActionParametersAttribute()
        {
            Order = 1;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                var parameters = descriptor.MethodInfo.GetParameters();

                foreach (var parameter in parameters)
                {
                    var argument = context.ActionArguments.ContainsKey(parameter.Name)
                        ? context.ActionArguments[parameter.Name]
                        : null;

                    EvaluateValidationAttributes(parameter, argument, context.ModelState);
                }
            }

            base.OnActionExecuting(context);
        }

        private void EvaluateValidationAttributes(ParameterInfo parameter, object argument, ModelStateDictionary modelState)
        {
            var validationAttributes = parameter.CustomAttributes;

            foreach (var attributeData in validationAttributes)
            {
                var attributeInstance = parameter.GetCustomAttribute(attributeData.AttributeType);

                if (attributeInstance is ValidationAttribute validationAttribute)
                {
                    var isValid = validationAttribute.IsValid(argument);
                    if (!isValid)
                    {
                        modelState.AddModelError(parameter.Name, validationAttribute.FormatErrorMessage(parameter.Name));
                    }
                }
            }
        }
    }
}
