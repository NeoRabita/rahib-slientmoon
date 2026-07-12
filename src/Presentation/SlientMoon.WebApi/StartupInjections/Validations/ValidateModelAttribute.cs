using Microsoft.AspNetCore.Mvc.Filters;

namespace SlientMoon.WebApi.StartupInjections.Validations
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
          
        }
    }
}