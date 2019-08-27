namespace Fanex.NAP.Core.Web.Attribute
{
    using Fanex.NAP.Client.Resources;
    using Fanex.NAP.Core.Web.Session;
    using System;
    using System.Web.Mvc;
    using System.Web.Security;

    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class NapSuperAuthAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.HttpContext == null)
            {
                return;
            }

            if (!CheckSessionAuthorization())
            {
                var response = filterContext.HttpContext.Response;

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    response.Buffer = true;
                    response.StatusCode = 403; // Unauthorized status code
                    response.Write("<center><b>" + ViewResources.UnauthorizeMsg + "</b></center>");
                    response.Flush();
                    response.Close();
                    response.End();
                }
                else
                {
                    var loginUrl = !string.IsNullOrWhiteSpace(FormsAuthentication.LoginUrl) ? FormsAuthentication.LoginUrl : "~/userlogin/login";
                    filterContext.Result = new RedirectResult(loginUrl);
                }
            }
        }

        protected bool CheckSessionAuthorization()
        {
            var session = new NapSession();

            return !string.IsNullOrEmpty(session?.UserName) && session?.UserId > 0;
        }
    }
}