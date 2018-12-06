namespace Fanex.NAP.Client.AspNetMVC.Attributes
{
    using Fanex.NAP.Client.AspNetMVC.Auth;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.Clients.Resources;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Configuration;
    using System;
    using System.Net;
    using System.Runtime.Caching;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class NapAuthAttribute : AuthorizeAttribute
    {
        private const string UnauthorizeUrl = "~/Admin/Common/Unauthorize";
        private static readonly ObjectCache Cache = MemoryCache.Default;
        private readonly IUserService userService;
        private readonly INapSettings napSettings;
        private Constant.Policy policy;

        public NapAuthAttribute() : this(null, null)
        {
        }

        internal NapAuthAttribute(IUserService userService, INapSettings napSettings)
        {
            this.userService = userService ?? new UserService();
            this.napSettings = napSettings ?? NapClientManager.NapSettings;
        }

        public string FunctionKey { get; set; }

        public bool PermissionCache { get; set; }

        public int CacheDuration { get; set; }

        public Constant.Policy Policy
        {
            get
            {
                return policy == Constant.Policy.None ? Constant.Policy.Read : policy;
            }

            set
            {
                policy = value;
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.HttpContext == null)
            {
                this.CloseConnection(filterContext);

                return;
            }

            var principal = filterContext.HttpContext.User as NapPrincipal ?? new NapPrincipal(filterContext.HttpContext);

            if (principal.AuthInfo == null)
            {
                CloseConnection(filterContext);

                return;
            }

            if (!IsAuthenticated(principal.AuthInfo.UserId))
            {
                CloseConnection(filterContext);
            }
        }

        protected bool IsAuthenticated(int userId)
        {
            if (userId <= 0)
            {
                return false;
            }

            int userPolicy = 0;

            if (PermissionCache)
            {
                var cacheKey = string.Format("{0}_{1}_{2}_{3}", napSettings.AccessKey, userId, napSettings.SiteId, FunctionKey);
                var cachedPolicy = Cache.Get(cacheKey);

                if (cachedPolicy == null)
                {
                    userPolicy = userService.GetPermission(userId, napSettings.SiteId, FunctionKey);
                    Cache.Set(cacheKey, userPolicy, DateTime.Now.AddSeconds(CacheDuration));
                }
                else
                {
                    userPolicy = (int)cachedPolicy;
                }
            }
            else
            {
                userPolicy = userService.GetPermission(userId, napSettings.SiteId, FunctionKey);
            }

            return userPolicy >= (int)Policy;
        }

        protected void CloseConnection(AuthorizationContext filterContext)
        {
            if (filterContext == null
                || filterContext.HttpContext == null
                || filterContext.HttpContext.Request == null)
            {
                SignOut();
                return;
            }

            HttpResponseBase response = filterContext.HttpContext.Response;

            if (response == null)
            {
                this.SignOut();
                return;
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                HandleForbiddenAjaxRequest(filterContext, response);
            }
            else if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                this.SignOut();
            }
            else
            {
                string settingUrl = napSettings.UnauthorizeUrl;
                filterContext.Result = new RedirectResult(string.IsNullOrWhiteSpace(settingUrl) ? UnauthorizeUrl : settingUrl);
            }
        }

        protected virtual void HandleForbiddenAjaxRequest(AuthorizationContext filterContext, HttpResponseBase response)
        {
            var messageContent = string.Format("<center><b>{0}</b></center>", ViewResources.UnauthorizeMsg);

            filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.Forbidden, messageContent);
            response.Buffer = true;
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            response.Write(messageContent);
            response.Flush();
            response.Close();
            response.End();
        }

        protected void SignOut()
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }
    }
}