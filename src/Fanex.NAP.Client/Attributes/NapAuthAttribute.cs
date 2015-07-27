namespace Fanex.NAP.Client.Attributes;

using System;
using System.Linq;
using Configuration;
using Fanex.NAP.Client;
using Helpers.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Services;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class NapAuthAttribute : Attribute, IAuthorizationFilter
{
    public string FunctionKey { get; set; }

    public Constant.Policy Policy { get; set; }

    public int CacheDuration { get; set; } = 60;
    
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

        if (!UserHasValidFunctionPolicy(context))
        {
            return;
        }
        var napSettings = context.HttpContext.RequestServices.GetRequiredService<INapSettings>();
        var unauthorizeUrl = napSettings.UnauthorizeUrl;
        context.Result = new RedirectResult(string.IsNullOrEmpty(unauthorizeUrl)
            ? "~/Unauthorize"
            : unauthorizeUrl);
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

    private bool UserHasValidFunctionPolicy(AuthorizationFilterContext context)
    {
        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
        var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
        var napSettings = context.HttpContext.RequestServices.GetRequiredService<INapSettings>();
        var currentSession = context.HttpContext.RequestServices.GetRequiredService<INapSession>();
        var cacheKey = $"{currentSession.UserId}_{napSettings.SiteId}_{FunctionKey}";
        var policy = cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(CacheDuration);
            return userService.GetPermission(currentSession.UserId, napSettings.SiteId, FunctionKey);
        });

        return policy < (int)Policy;
    }
}