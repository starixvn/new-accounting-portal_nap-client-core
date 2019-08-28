namespace Fanex.NAP.Client.AspNetCore.Attributes
{
    using System;
    using Fanex.NAP.Client.Helpers.Web;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NapSuperAuthAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly INapSession napSession;

        public NapSuperAuthAttribute() : this(null)
        {
        }

        internal NapSuperAuthAttribute(INapSession napSession)
        {
            this.napSession = napSession ?? NapClientManager.NapSession;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context?.HttpContext == null)
            {
                RequestHelper.CloseConnection(context);
                return;
            }

            if (napSession == null || napSession.UserId <= 0 || string.IsNullOrEmpty(napSession.UserName))
            {
                RequestHelper.CloseConnection(context);
            }
        }
    }
}