namespace Fanex.NAP.Client.AspNetMVC.Auth
{
    using System;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Security;

    public class NapPrincipal : IPrincipal
    {
        public NapPrincipal(IIdentity identity, AuthInfo authInfo)
        {
            Identity = identity;
            AuthInfo = authInfo;
        }

        public AuthInfo AuthInfo { get; }

        public IIdentity Identity { get; }

        public NapPrincipal(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            if (HttpContext.Current.User.Identity is FormsIdentity && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var httpCookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];

                if (httpCookie == null)
                {
                    return;
                }

                var ticket = FormsAuthentication.Decrypt(httpCookie.Value);

                if (ticket?.Expired != false || string.IsNullOrEmpty(ticket.UserData))
                {
                    return;
                }

                AuthInfo = AuthInfo.CreateUserData(ticket.UserData);
                Identity = new FormsIdentity(ticket);
            }
        }

        public bool IsInRole(string role)
        {
            return true;
        }
    }
}