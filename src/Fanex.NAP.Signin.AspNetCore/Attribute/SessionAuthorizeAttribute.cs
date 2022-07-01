using Fanex.NAP.Client.AspNetCore.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Fanex.NAP.Signin.AspNetCore.Attribute
{
    /// <summary>
    /// Super authorize attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
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

            if (filterContext.ActionDescriptor is ControllerActionDescriptor controllerDescriptor
                && controllerDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute)))
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