using Microsoft.AspNetCore.Mvc.Filters;

public class LogActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<LogActionFilter>>();
        var actionName = context.ActionDescriptor.DisplayName;

        logger.LogInformation("Action {Action} execution started with arguments: {@Arguments}",
            actionName, context.ActionArguments);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<LogActionFilter>>();
        var actionName = context.ActionDescriptor.DisplayName;

        if (context.Exception != null)
        {
            logger.LogError(context.Exception, "Error in action {Action}", actionName);
        }
        else
        {
            logger.LogInformation("Action {Action} completed successfully", actionName);
        }
    }
}
