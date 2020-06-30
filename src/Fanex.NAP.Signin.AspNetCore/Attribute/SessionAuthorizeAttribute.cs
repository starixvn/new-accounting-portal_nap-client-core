using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Fanex.NAP.Client.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Fanex.NAP.Signin.AspNetCore
{
    /// <summary>
    /// Super authorize attribute.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SessionAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// Authorize action handler.
        /// </summary>
        /// <param name="filterContext">Filter context.</param>
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            bool skipAuthorization = SkipAuthorization(filterContext);

            if (skipAuthorization)
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

            var controllerDescriptor = filterContext.ActionDescriptor as ControllerActionDescriptor;
            if (controllerDescriptor != null && controllerDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute)))
            {
                return true;
            }

            return false;
        }

        protected bool SessionIsValid()
        {
            var accessor = new HttpContextAccessor();
            var session = new NapSession(accessor);
            return !string.IsNullOrEmpty(session.UserName) && session.UserId > 0;
        }
    }
}