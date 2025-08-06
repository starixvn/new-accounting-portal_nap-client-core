namespace Fanex.NAP.Client.Attributes
{
    using System;
    using System.Linq;
    using Configuration;
    using Helpers.Business;
    using Helpers.Web;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;
    using Services;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class NapAuthAttribute : Attribute, IAuthorizationFilter
    {
        public string FunctionKey { get; set; }

        public Constant.Policy Policy { get; set; }

        public bool PermissionCache { get; set; }

        public int CacheDuration { get; set; } = 60;

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (IsOptionsMethod(filterContext))
            {
                return;
            }

            if (SkipAuthorization(filterContext))
            {
                return;
            }

            if (!IsAuthenticated(filterContext))
            {
                return;
            }

            if (!UserHasValidFunctionPolicy(filterContext))
            {
                return;
            }

            var napSettings = filterContext.HttpContext.RequestServices.GetRequiredService<INapSettings>();
            var unauthorizeUrl = napSettings.UnauthorizeUrl;
            filterContext.Result = new RedirectResult(string.IsNullOrEmpty(unauthorizeUrl)
                ? "~/Unauthorize"
                : unauthorizeUrl);
        }

        private static bool IsOptionsMethod(AuthorizationFilterContext filterContext)
        {
            return filterContext?.HttpContext?.Request?.Method == "OPTIONS";
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
            var identity = filterContext?.HttpContext?.User?.Identity;

            return identity != null && identity.IsAuthenticated;
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
}