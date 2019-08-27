namespace Fanex.NAP.Client.AspNetCore.Attributes
{
    using System;
    using System.Net;
    using Fanex.NAP.Client.Resources;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;

    internal static class RequestHelper
    {
        public static void CloseConnection(AuthorizationFilterContext context)
        {
            if (IsAjaxRequest(context.HttpContext.Request))
            {
                var response = context.HttpContext.Response;
                var messageContent = $"<center><b>{ViewResources.UnauthorizeMsg}</b></center>";
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                response.WriteAsync(messageContent);
            }
            else
            {
                context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }

        public static bool IsAjaxRequest(HttpRequest request)
           => string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
              string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal);
    }
}