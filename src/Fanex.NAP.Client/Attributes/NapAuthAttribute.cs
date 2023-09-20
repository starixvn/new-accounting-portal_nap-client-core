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
    using Services;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class NapAuthAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IMemoryCache cache;
        private int cacheDuration;
        private readonly IUserService userService;
        private readonly INapSettings napSettings;
        private readonly INapSession napSession;

        public NapAuthAttribute() : this(null, null, null, null)
        {
        }

        internal NapAuthAttribute(IUserService userService, INapSettings napSettings, INapSession napSession, IMemoryCache cache)
        {
            this.userService = userService ?? new UserService();
            this.napSettings = napSettings ?? NapClientManager.NapSettings;
            this.napSession = napSession ?? NapClientManager.NapSession;
            this.cache = cache ?? new MemoryCache(new MemoryCacheOptions());
        }

        public string FunctionKey { get; set; }

        public Constant.Policy Policy { get; set; }

        public bool PermissionCache { get; set; }

        public int CacheDuration
        {
            get => cacheDuration == 0 ? napSettings.CacheDuration : cacheDuration;
            set => cacheDuration = value;
        }

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

            if (!UserHasValidFunctionPolicy(napSession))
            {
                return;
            }

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

        private bool UserHasValidFunctionPolicy(INapSession currentSession)
        {
            int policy;

            if (PermissionCache)
            {
                var cacheKey = $"{currentSession.UserId}_{napSettings.SiteId}_{FunctionKey}";
                var cachedPolicy = cache.Get(cacheKey);

                if (cachedPolicy == null)
                {
                    policy = userService.GetPermission(currentSession.UserId, napSettings.SiteId, FunctionKey);
                    cache.Set(cacheKey, policy, DateTime.Now.AddSeconds(CacheDuration));
                }
                else
                {
                    policy = (int)cachedPolicy;
                }
            }
            else
            {
                policy = userService.GetPermission(currentSession.UserId, napSettings.SiteId, FunctionKey);
            }

            return policy < (int)Policy;
        }
    }
}