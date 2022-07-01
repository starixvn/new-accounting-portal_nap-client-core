namespace Fanex.NAP.Client.AspNetCore.Attributes
{
    using System;
    using Fanex.NAP.Client.AspNetCore.Session;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.Core.Business;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Caching.Memory;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
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

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context?.HttpContext == null)
            {
                RequestHelper.CloseConnection(context);
                return;
            }

            if (UserHasValidFunctionPolicy(napSession))
            {
                var unauthorizeUrl = napSettings.UnauthorizeUrl;
                context.Result = new RedirectResult(string.IsNullOrEmpty(unauthorizeUrl)
                    ? "~/Admin/Common/Unauthorize"
                    : unauthorizeUrl);
            }
        }

        private bool UserHasValidFunctionPolicy(INapSession currentSession)
        {
            int policy = 0;

            if (PermissionCache)
            {
                var cacheKey = string.Format("{0}_{1}_{2}", currentSession.UserId, napSettings.SiteId, FunctionKey);
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