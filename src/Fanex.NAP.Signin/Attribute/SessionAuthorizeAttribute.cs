namespace Fanex.NAP.SignIn.Attribute;

using System;
using System.Linq;
using Fanex.NAP.Client.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class SessionAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext filterContext)
    {
        if (SkipAuthorization(filterContext))
        {
            return;
        }

        if (!IsAuthenticated(filterContext))
        {
            return;
        }

        if (SessionIsValid())
        {
            return;
        }

        RequestHelper.CloseConnection(filterContext);
    }

    private static bool SkipAuthorization(AuthorizationFilterContext filterContext)
    {
        if (filterContext.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute)))
        {
            return true;
        }

        return filterContext.ActionDescriptor is ControllerActionDescriptor controllerDescriptor
               && controllerDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute));
    }

    private static bool IsAuthenticated(AuthorizationFilterContext filterContext)
    {
        var identity = filterContext?.HttpContext.User.Identity;

        return identity != null && identity.IsAuthenticated;
    }

    protected bool SessionIsValid()
    {
        var accessor = new HttpContextAccessor();
        var session = new NapSession(accessor);
        return !string.IsNullOrEmpty(session.UserName) && session.UserId > 0;
    }
}