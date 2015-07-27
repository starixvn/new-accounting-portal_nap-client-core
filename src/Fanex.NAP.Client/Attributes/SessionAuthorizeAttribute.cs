using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fanex.NAP.Client.Attributes;

using System;
using Microsoft.Extensions.DependencyInjection;
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class SessionAuthorizeAttribute: AuthorizeAttribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (IsOptionsMethod(context))
        {
            return;
        }

        if (SkipAuthorization(context))
        {
            return;
        }

        if (!IsAuthenticated(context))
        {
            return;
        }

        if (SessionIsValid(context))
        {
            return;
        }

        RequestHelper.CloseConnection(context);
    }

    private static bool IsOptionsMethod(AuthorizationFilterContext context)
    {
        return context?.HttpContext?.Request?.Method == "OPTIONS";
    }

    private static bool SkipAuthorization(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute)))
        {
            return true;
        }

        return context.ActionDescriptor is ControllerActionDescriptor controllerDescriptor
               && controllerDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute));
    }

    private static bool IsAuthenticated(AuthorizationFilterContext context)
    {
        var identity = context?.HttpContext?.User?.Identity;

        return identity is { IsAuthenticated: true };
    }

    private static bool SessionIsValid(AuthorizationFilterContext context)
    {
        var currentSession = context.HttpContext.RequestServices.GetRequiredService<INapSession>();
        return !string.IsNullOrEmpty(currentSession.UserName) && currentSession.UserId > 0;
    }
}